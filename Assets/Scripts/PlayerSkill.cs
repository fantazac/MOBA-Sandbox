using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class PlayerSkill : MonoBehaviour
{

    public float cooldown;
    public Sprite skillImage;
    public float castTime;

    protected WaitForSeconds delayCastTime;

    protected PlayerMovement playerMovement;
    protected InputManager inputManager;
    protected int skillId;

    public delegate void SkillActivatedHandler(int skillId, Vector3 mousePositionOnTerrain);
    public event SkillActivatedHandler SkillActivated;

    public delegate void SkillFinishedHandler(int skillId);
    public event SkillFinishedHandler SkillFinished;

    protected virtual void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    protected void SkillBeingUsed(int skillId, Vector3 mousePositionOnTerrain)
    {
        SkillActivated(skillId, mousePositionOnTerrain);
    }

    protected void SkillDone(int skillId)
    {
        SkillFinished(skillId);
    }

    public virtual void ActivateSkill() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffect(Vector3 mousePositionOnTerrain) { yield return null; }
    protected virtual IEnumerator SkillCastTime() { yield return null; }
    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }

}
