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

    public List<PlayerSkill> skills;

    [HideInInspector]
    public Actions nextAction = Actions.NONE;
    protected int nextSkillId;
    protected Vector3 nextMousePosition;

    protected PlayerSkill passive;

    protected bool infoSent = false;
    protected bool isNewPlayer = true;

    protected override void Start()
    {
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;

        PlayerStats.health.OnDeath += OnDeath;

        PlayerInput.OnPressedD += DoDamageToPlayer;
        PlayerInput.OnPressedF += DoHealToPlayer;
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null)
            {
                if (ps.isPassive)
                {
                    passive = ps;
                }
                if (PhotonView.isMine)
                {
                    ps.SkillFinished += UseNextAction;
                }
            }
        }

        base.Start();
    }

    public PlayerSkill GetPassiveSkill()
    {
        return passive;
    }

    protected virtual void InitialiseStats() { }
    protected virtual void AdjustStats() { }

    protected void DoDamageToPlayer()
    {
        PlayerStats.health.DamageTargetOnServer(10);
    }

    protected void DoHealToPlayer()
    {
        PlayerStats.health.HealTargetOnServer(10);
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

    public void SendInfo()
    {
        PhotonView.RPC("SendInfoToNewPlayer", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    protected void SendInfoToNewPlayer()
    {
        if (PhotonView.isMine)
        {
            PhotonView.RPC("ReceiveInfoForNewPlayer", PhotonTargets.AllViaServer, GetComponent<Health>().currentHealth, transform.position);
        }
    }

    [PunRPC]
    protected void ReceiveInfoForNewPlayer(float currentHealth, Vector3 position)
    {
        //verify if this is useful with multiple clients
        if (isNewPlayer)
        {
            isNewPlayer = false;
            transform.position = position;
            PlayerMovement.NotifyPlayerMoved();
            GetComponent<Health>().currentHealth = currentHealth;
        }
    }

    public void SetPlayerId(int playerId)
    {
        PhotonView.RPC("SetPlayerOnNetwork", PhotonTargets.AllBufferedViaServer, playerId);
    }

    public virtual void SetBackMovementAfterSkillWithoutCastTime()
    {
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SetBackMovementAfterSkillWithoutCastTime for this object: " + gameObject.name);
        }

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
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SetNextAction for this object: " + gameObject.name);
        }

        nextAction = action;
        nextSkillId = skillId;
        nextMousePosition = mousePosition;
    }

    //Unused?
    public void UseNextActionAfterBasicAttack()
    {
        if (PhotonView.isMine)
        {
            UseNextAction();
        }
    }

    //Should only be called if PhotonView.isMine, aka the local player's character
    protected void UseNextAction()
    {
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check UseNextAction for this object: " + gameObject.name);
        }

        if (!PlayerStats.health.IsDead() && nextAction != Actions.NONE)
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
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SendActionToServer for this object: " + gameObject.name);
        }

        if (CanUseSkill(skillId) && (!skills[skillId].HasCastTime() || !infoSent))
        {
            SetNextActionAfterCastingSkillWithCastTime(skillId);
            if (skills[skillId].isATargetSkill)
            {
                PhotonView.RPC("CancelMovementOnServer", PhotonTargets.AllViaServer);
                SendSkillInfoToServer(skillId, Vector3.zero, PlayerMouseSelection.HoveredObject.GetComponent<Player>().PlayerId);
            }
            else
            {
                SendSkillInfoToServer(skillId, mousePosition, -1);
            }
        }
        else
        {
            PhotonView.RPC("CancelMovementOnServer", PhotonTargets.AllViaServer);
            SetNextAction(action, skillId, mousePosition);
        }
    }

    [PunRPC]
    protected void CancelMovementOnServer()
    {
        PlayerMovement.CancelMovement();
    }

    protected void SendSkillInfoToServer(int skillId, Vector3 mousePosition, int playerId)
    {
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SendSkillInfoToServer for this object: " + gameObject.name);
        }

        //change to AllBufferedViaServer when prediction is good (ex. ezreal ult server position has to be calculated)
        if (playerId == -1)
        {
            PhotonView.RPC("UseSkillFromServer", PhotonTargets.AllViaServer, skillId, mousePosition);
        }
        else
        {
            PhotonView.RPC("UseSkillFromServer", PhotonTargets.AllViaServer, skillId, Vector3.right * playerId);
        }
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

    public virtual void ProjectileHitEnemyTarget() { }
    public virtual void ProjectileHitAllyTarget() { }
}
