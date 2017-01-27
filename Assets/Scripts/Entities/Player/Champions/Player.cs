using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Player : PlayerBase
{
    protected int playerId;

    [HideInInspector]
    public GameObject healthBar;

    [HideInInspector]
    public Vector3 halfHeight;

    public int PlayerId { get { return playerId; } }

    [HideInInspector]
    public Health health;
    [HideInInspector]
    public float movementSpeed;
    [HideInInspector]
    public float range;

    public List<PlayerSkill> skills;

    [HideInInspector]
    public Actions nextAction = Actions.NONE;
    protected int nextSkillId;
    protected Vector3 nextMousePosition;

    protected bool infoSent = false;

    protected override void Start()
    {
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;

        health = GetComponent<Health>();
        health.OnDeath += OnDeath;

        PlayerInput.OnPressedD += DoDamageToPlayer;
        PlayerInput.OnPressedF += DoHealToPlayer;
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null)
            {
                ps.SkillFinished += UseNextAction;
            }
        }

        base.Start();
    }

    protected virtual void InitialiseStats() { }
    protected virtual void AdjustStats() { }

    protected void DoDamageToPlayer()
    {
        health.DamageTargetOnServer(10);
    }

    protected void DoHealToPlayer()
    {
        health.HealTargetOnServer(10);
    }

    protected void OnDeath()
    {
        healthBar.SetActive(false);
        //stop all skills (cancel cast time of current skill, stop skills with no cast time), 
        //then stop movement, then start death animation
        PlayerMovement.StopMovement();
        DeathAnimation();
    }

    protected void DeathAnimation()
    {
        StartCoroutine(SinkThroughFloorOnDeath());
    }

    protected IEnumerator SinkThroughFloorOnDeath()
    {
        while (transform.position.y > -1)
        {
            transform.position = transform.position - Vector3.up * 0.06f;

            yield return null;
        }
    }

    public void SetPlayerId(int playerId)
    {
        PhotonView.RPC("SetPlayerOnNetwork", PhotonTargets.AllBufferedViaServer, playerId);
    }

    public virtual void SetBackMovementAfterSkillWithoutCastTime()
    {
        if (nextAction == Actions.NONE)
        {
            if (PlayerNormalMovement.WasMovingBeforeSkill())
            {
                PlayerNormalMovement.ActivateMovementTowardsPoint();
            }
            else if (PlayerAttackMovement.WasMovingBeforeSkill())
            {
                PlayerAttackMovement.ActivateMovementTowardsUnfriendlyTarget();
            }
        }
    }

    protected void SetNextAction(Actions action, int skillId, Vector3 mousePosition)
    {
        nextAction = action;
        nextSkillId = skillId;
        nextMousePosition = mousePosition;
    }

    protected void UseNextAction()
    {
        if (!Player.health.IsDead())
        {
            if (nextAction == Actions.SKILL)
            {
                SendActionToServer(nextAction, nextSkillId, nextMousePosition);
            }
            else if (nextAction == Actions.ATTACK)
            {
                nextAction = Actions.NONE;
                PlayerAttackMovement.ActivateMovementTowardsUnfriendlyTarget();
            }
            else if (nextAction == Actions.MOVE)
            {
                nextAction = Actions.NONE;
                PlayerNormalMovement.ActivateMovementTowardsPoint();
            }
        }
    }

    public void CancelSkillIfUncastable(int skillId)
    {
        if (nextAction == Actions.SKILL && nextSkillId == skillId)
        {
            nextAction = Actions.NONE;
        }
    }

    [PunRPC]
    protected void SetPlayerOnNetwork(int playerId)
    {
        this.playerId = playerId;
    }

    public virtual void SendActionToServer(Actions action, int skillId, Vector3 mousePosition)
    {
        if (CanUseSkill(skillId) && (!skills[skillId].HasCastTime() || !infoSent))
        {
            SetNextActionAfterCastingSkillWithCastTime(skillId);
            
            SendSkillInfoToServer(skillId, mousePosition);
        }
        else
        {
            PlayerMovement.CancelMovement();
            SetNextAction(action, skillId, mousePosition);
        }
    }

    protected void SendSkillInfoToServer(int skillId, Vector3 mousePosition)
    {
        //change to AllBufferedViaServer when prediction is good (ex. ezreal ult server position has to be calculated)
        PhotonView.RPC("UseSkillFromServer", PhotonTargets.AllViaServer, skillId, mousePosition);
    }

    protected void SetNextActionAfterCastingSkillWithCastTime(int skillId)
    {
        if (skills[skillId].HasCastTime())
        {
            if (PlayerNormalMovement.WasMovingBeforeSkill())
            {
                nextAction = Actions.MOVE;
            }
            else if (PlayerAttackMovement.WasMovingBeforeSkill())
            {
                nextAction = Actions.ATTACK;
            }
            else
            {
                nextAction = Actions.NONE;
            }
            infoSent = true;
        }
    }

    [PunRPC]
    protected virtual void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        infoSent = false;
        skills[skillId].InfoReceivedFromServerToUseSkill(mousePositionOnCast);
    }

    [PunRPC]
    protected void CancelSkillFromServer(int skillId)
    {
        skills[skillId].InfoReceivedFromServerToCancelSkill();
    }

    protected bool CanUseSkill(int skillId)
    {
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null && ps.skillIsActive && skills[skillId].HasCastTime())
            {
                return false;
            }
        }
        return true;
    }
}
