using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class PlayerSkill : MonoBehaviour
{
    [HideInInspector]
    public AbilityType AbilityType { get; protected set; }

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

    protected RaycastHit hit;
    protected Vector3 mousePositionOnCast;

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

    public void ActivateSkill()
    {
        usingSkillFromThisView = true;
        playerMovement.PhotonView.RPC("UseSkillFromServer", PhotonTargets.All, skillId, hit.point + playerMovement.halfHeight);
    }

    public void CancelSkill()
    {
        playerMovement.PhotonView.RPC("CancelSkillFromServer", PhotonTargets.All, skillId);
    }

    protected IEnumerator PingDelay()
    {
        yield return delayPing;

        UseSkillFromServer();
    }

    public void InfoReceivedFromServerToUseSkill(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        if (usingSkillFromThisView)
        {
            delayPing = new WaitForSeconds((float)PhotonNetwork.GetPing() * 0.001f);
            StartCoroutine(PingDelay());
        }
        else
        {
            UseSkillFromServer();
        }
    }

    public void InfoReceivedFromServerToCancelSkill()
    {
        CancelSkillFromServer();
    }

    public void SetSkillId(int skillId)
    {
        this.skillId = skillId;
    }

    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }

    protected virtual void UseSkillFromServer() { }
    protected virtual void CancelSkillFromServer() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffectWithCastTime() { yield return null; }
}
