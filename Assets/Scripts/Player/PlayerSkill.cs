using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class PlayerSkill : MonoBehaviour
{

    public float cooldown;
    public Sprite skillImage;
    public float castTime;
    public bool canMoveWhileCasting = false;
    public bool canBeCancelled = false;
    public bool canRotateWhileCasting = false;
    public bool cooldownStartsOnCast = true;
    public bool continueMovementAfterCast = true;
    public List<PlayerSkill> castableSpellsWhileActive;
    [HideInInspector]
    public bool skillActive = false;

    protected bool usingSkillFromThisView = false;
    protected WaitForSeconds delayPing;

    protected WaitForSeconds delayCastTime;

    protected PlayerMovement playerMovement;
    protected int skillId;

    public delegate void SkillStartedHandler();
    public event SkillStartedHandler SkillStarted;

    public delegate void SkillFinishedHandler();
    public event SkillFinishedHandler SkillFinished;

    public delegate void SetCooldownHandler(int skillId);
    public event SetCooldownHandler SetCooldown;

    protected virtual void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    protected void SkillBegin()
    {
        skillActive = true;
        if (SkillStarted != null)
        {
            SkillStarted();
        }
    }

    protected void SkillDone()
    {
        skillActive = false;
        if (SkillFinished != null && !canMoveWhileCasting)
        {
            SkillFinished();
        }
        if(SetCooldown != null)
        {
            SetCooldown(skillId);
        }
    }

    protected IEnumerator PingDelay(Vector3 mousePositionOnCast)
    {
        yield return delayPing;

        UseSkill(mousePositionOnCast);
    }

    protected void InfoReceivedFromServer(Vector3 mousePositionOnCast)
    {
        if (usingSkillFromThisView)
        {
            delayPing = new WaitForSeconds((float)PhotonNetwork.GetPing() * 0.001f);
            StartCoroutine(PingDelay(mousePositionOnCast));
        }
        else
        {
            UseSkill(mousePositionOnCast);
        }
    }

    public virtual void ActivateSkill() { }
    public virtual void CancelSkill() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffect(Vector3 mousePositionOnTerrain) { yield return null; }
    protected virtual IEnumerator SkillEffectWithCastTime(Vector3 mousePositionOnTerrain) { yield return null; }
    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }
    protected virtual void UseSkill(Vector3 mousePositionOnCast) { }
}
