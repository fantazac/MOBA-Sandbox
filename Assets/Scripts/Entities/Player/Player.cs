﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    [SerializeField]
    private int playerId;

    [HideInInspector]
    public GameObject healthBar;

    [HideInInspector]
    public Vector3 halfHeight;

    public int PlayerId { get { return playerId; } }

    [HideInInspector]
    public Health health;

    public float movementSpeed = 325;
    public float range;

    public List<PlayerSkill> skills;

    [HideInInspector]
    public Actions nextAction = Actions.NONE;
    private int nextSkillId;
    private Vector3 nextMousePosition;

    private bool infoSent = false;

    protected override void Start()
    {
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;

        health = GetComponent<Health>();
        health.OnDeath += OnDeath;

        PlayerInput.OnPressedD += DoDamageToPlayer;
        PlayerInput.OnPressedF += DoHealToPlayer;
        foreach (PlayerSkill ps in skills)
        {
            if(ps != null)
            {
                ps.SkillFinished += UseNextAction;
            }
        }

        base.Start();
    }

    private void DoDamageToPlayer()
    {
        health.DamageTargetOnServer(10);
    }

    private void DoHealToPlayer()
    {
        health.HealTargetOnServer(10);
    }

    private void OnDeath()
    {
        healthBar.SetActive(false);
        //stop all skills (cancel cast time of current skill, stop skills with no cast time), 
        //then stop movement, then start death animation
        PlayerMovement.StopMovement();
        DeathAnimation();
    }

    private void DeathAnimation()
    {
        StartCoroutine(SinkThroughFloorOnDeath());
    }

    private IEnumerator SinkThroughFloorOnDeath()
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

    private void SetNextAction(Actions action, int skillId, Vector3 mousePosition)
    {
        nextAction = action;
        nextSkillId = skillId;
        nextMousePosition = mousePosition;
    }

    private void UseNextAction()
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

    public void SetBackMovementAfterDash()
    {
        if(nextAction == Actions.NONE)
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

    public void CancelSkillIfUncastable(int skillId)
    {
        if(nextAction == Actions.SKILL && nextSkillId == skillId)
        {
            nextAction = Actions.NONE;
        }
    }

    [PunRPC]
    private void SetPlayerOnNetwork(int playerId)
    {
        this.playerId = playerId;
    }

    public void SendActionToServer(Actions action, int skillId, Vector3 mousePosition)
    {
        Debug.Log(1);
        if (CanUseSkill(skillId) && (!skills[skillId].HasCastTime() || !infoSent))
        {
            Debug.Log(2);
            if (skills[skillId].HasCastTime())
            {
                Debug.Log(3);
                if (PlayerNormalMovement.WasMovingBeforeSkill())
                {
                    Debug.Log(4);
                    nextAction = Actions.MOVE;
                }
                else if (PlayerAttackMovement.WasMovingBeforeSkill())
                {
                    Debug.Log(5);
                    nextAction = Actions.ATTACK;
                }
                else
                {
                    Debug.Log(6);
                    nextAction = Actions.NONE;
                }
                infoSent = true;
            }
            Debug.Log(7);
            //change to AllBufferedViaServer when prediction is good (ex. ezreal ult server position has to be calculated)
            PhotonView.RPC("UseSkillFromServer", PhotonTargets.AllViaServer, skillId, mousePosition);
        }
        else
        {
            Debug.Log(8);
            PlayerMovement.CancelMovement();
            SetNextAction(action, skillId, mousePosition);
        }
    }

    [PunRPC]
    protected void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        infoSent = false;
        skills[skillId].InfoReceivedFromServerToUseSkill(mousePositionOnCast);
    }

    [PunRPC]
    protected void CancelSkillFromServer(int skillId)
    {
        skills[skillId].InfoReceivedFromServerToCancelSkill();
    }

    private bool CanUseSkill(int skillId)
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
