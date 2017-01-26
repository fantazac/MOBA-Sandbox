using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class PlayerSkill : MonoBehaviour
{
    [HideInInspector]
    public AbilityType AbilityType { get; protected set; }

    private CastTimeBarManager castTimeBar;

    protected const float DIVISION_FACTOR = 100f;

    public Sprite skillImage;

    public float cooldown;
    public float castTime;
    public bool canMoveWhileCasting = false;
    public bool canBeCancelled = false;
    public bool canRotateWhileCasting = false;
    public bool cooldownStartsOnCast = true;
    //doesnt work
    public List<PlayerSkill> uncastableSpellsWhileActive;
    [HideInInspector]
    public bool skillIsActive = false;
    protected string skillName = "TEMPORARY";

    [HideInInspector]
    public UISkill uiSkill;

    protected WaitForSeconds delayCastTime;

    protected PlayerMovement playerMovement;
    protected int skillId;

    protected RaycastHit hit;
    protected Vector3 mousePositionOnCast;

    public delegate void SkillStartedHandler();
    public event SkillStartedHandler SkillStarted;

    public delegate void SkillFinishedHandler();
    public event SkillFinishedHandler SkillFinished;

    public delegate void SetCooldownOnSkillStartedHandler(int skillId);
    public event SetCooldownOnSkillStartedHandler SetCooldownOnSkillStarted;

    public delegate void SetCooldownOnSkillFinishedHandler(int skillId);
    public event SetCooldownOnSkillFinishedHandler SetCooldownOnSkillFinished;

    protected virtual void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement.PhotonView.isMine)
        {
            castTimeBar = transform.parent.GetChild(1).gameObject.GetComponentInChildren<CastTimeBarManager>();
        }
    }

    public bool HasCastTime()
    {
        return castTime > 0;
    }

    protected void SkillBegin()
    {
        skillIsActive = true;
        if (SkillStarted != null)
        {
            SkillStarted();
        }
        if (SetCooldownOnSkillStarted != null)
        {
            SetCooldownOnSkillStarted(skillId);
        }
    }

    protected virtual void SkillDone()
    {
        skillIsActive = false;
        foreach (PlayerSkill uncastableSkill in uncastableSpellsWhileActive)
        {
            uncastableSkill.uiSkill.SetCastable();
        }
        if (SkillFinished != null)
        {
            SkillFinished();
        }
        if(SetCooldownOnSkillFinished != null)
        {
            SetCooldownOnSkillFinished(skillId);
        }
    }

    public void ActivateSkill()
    {
        playerMovement.Player.SendActionToServer("UseSkillFromServer", skillId, hit.point + playerMovement.halfHeight);
    }

    public void CancelSkill()
    {
        playerMovement.PhotonView.RPC("CancelSkillFromServer", PhotonTargets.AllViaServer, skillId);
    }

    public void InfoReceivedFromServerToUseSkill(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        UseSkillFromServer();
        foreach(PlayerSkill uncastableSkill in uncastableSpellsWhileActive)
        {
            uncastableSkill.uiSkill.SetUncastable();
            playerMovement.Player.CancelSkillIfUncastable(uncastableSkill.skillId);
        }
        if(castTimeBar != null && castTime > 0)
        {
            castTimeBar.gameObject.SetActive(true);
            castTimeBar.SetCastTimeBar(skillName, castTime);
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

    protected bool MouseIsOnTerrain(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity);
    }

    public virtual bool CanUseSkill(Vector3 mousePosition) { return false; }

    protected virtual void ModifyValues() { }
    protected virtual void UseSkillFromServer() { }
    protected virtual void CancelSkillFromServer() { }
    protected virtual IEnumerator SkillEffect() { yield return null; }
    protected virtual IEnumerator SkillEffectWithCastTime() { yield return null; }
}
