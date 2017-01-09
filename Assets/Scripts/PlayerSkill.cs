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
    [HideInInspector]
    public bool skillActive = false;

    protected WaitForSeconds delayCastTime;

    protected PlayerMovement playerMovement;
    protected int skillId;

    public delegate void SkillFinishedHandler();
    public event SkillFinishedHandler SkillFinished;

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
    }

    public virtual void ActivateSkill() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffect(Vector3 mousePositionOnTerrain) { yield return null; }
    protected virtual IEnumerator SkillEffectWithCastTime(Vector3 mousePositionOnTerrain) { yield return null; }
    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }
}
