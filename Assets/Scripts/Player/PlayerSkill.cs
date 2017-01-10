using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class PlayerSkill : MonoBehaviour
{

    public float cooldown;
    public Sprite skillImage;
    public float castTime;
    public bool canMoveWhileCasting = false;
    public bool canCastOtherSpellsWhileCasting = false;
    public bool canBeCancelled = false;
    public bool canRotateWhileCasting = false;
    public bool cooldownStartsOnCast = true;
    [HideInInspector]
    public bool skillActive = false;

    protected WaitForSeconds delayCastTime;

    protected PlayerMovement playerMovement;
    public int skillId;

    public delegate void SkillFinishedHandler();
    public event SkillFinishedHandler SkillFinished;

    public delegate void SetCooldownHandler(int skillId);
    public event SetCooldownHandler SetCooldown;

    protected virtual void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

    public virtual void ActivateSkill() { }
    public virtual void CancelSkill() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffect(Vector3 mousePositionOnTerrain) { yield return null; }
    protected virtual IEnumerator SkillEffectWithCastTime(Vector3 mousePositionOnTerrain) { yield return null; }
    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }
}
