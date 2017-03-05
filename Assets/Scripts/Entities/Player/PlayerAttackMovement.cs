using UnityEngine;
using System.Collections;

public class PlayerAttackMovement : PlayerBase
{
    private int lastNetworkTarget;

    public delegate void IsInRangeForBasicAttackHandler(int targetId);
    public event IsInRangeForBasicAttackHandler IsInRangeForBasicAttack;

    public delegate void IsInRangeForSkillHandler();
    public event IsInRangeForSkillHandler IsInRangeForSkill;

    protected override void Start()
    {
        lastNetworkTarget = -1;
        base.Start();
    }

    public void UseAttackMove()
    {
        lastNetworkTarget = PlayerMouseSelection.HoveredObject.GetComponent<Player>().PlayerId;
        ActivateMovementTowardsUnfriendlyTarget();
    }

    public void UseAttackMove(int targetId)
    {
        lastNetworkTarget = targetId;
        ActivateMovementTowardsUnfriendlyTarget();
    }

    public void ActivateMovementTowardsUnfriendlyTarget()
    {
        if (Player.nextAction == Actions.SKILL || Player.nextAction == Actions.MOVE)
        {
            Player.nextAction = Actions.ATTACK;
            PhotonView.RPC("StopAllCoroutinesOnServerForAttackMovement", PhotonTargets.AllViaServer);
        }
        else if (lastNetworkTarget != -1 && PlayerMovement.CanUseMovement())
        {
            PhotonView.RPC("MoveTowardsUnfriendlyTargetFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkTarget);
        }
    }

    [PunRPC]
    protected void StopAllCoroutinesOnServerForAttackMovement()
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
    }

    public bool WasMovingBeforeSkill()
    {
        return lastNetworkTarget != -1;
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        IsInRangeForSkill = null;
        lastNetworkTarget = -1;
        BasicAttack.CancelBasicAttack();
    }

    public bool IsInRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= PlayerStats.range;
    }

    [PunRPC]
    private void MoveTowardsUnfriendlyTargetFromServer(int enemyPlayerId)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        GameObject enemyPlayer = FindEnemyPlayer(enemyPlayerId);
        if(enemyPlayer != null)
        {
            SetMoveTowardsUnfriendlyTarget(enemyPlayer.transform);
        }
    }

    public GameObject FindEnemyPlayer(int enemyPlayerId)
    {
        GameObject enemyPlayer = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().PlayerId == enemyPlayerId)
            {
                enemyPlayer = player;
                break;
            }
        }

        return enemyPlayer;
    }

    public void SetMoveTowardsUnfriendlyTarget(Transform unfriendlyTarget, float range)
    {
        StopAllCoroutines();
        BasicAttack.CancelBasicAttack();
        IsInRangeForSkill = null;
        PlayerNormalMovement.StopMovement();
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsUnfriendlyTarget(unfriendlyTarget, range));
        PlayerOrientation.RotatePlayerUntilReachedTarget(unfriendlyTarget);
    }

    public void SetMoveTowardsUnfriendlyTarget(Transform unfriendlyTarget)
    {
        SetMoveTowardsUnfriendlyTarget(unfriendlyTarget, PlayerStats.range);
    }
    
    private IEnumerator MoveTowardsUnfriendlyTarget(Transform enemyTarget, float range)
    {
        Health enemyTargetHealth = enemyTarget.gameObject.GetComponent<Health>();
        
        while (enemyTarget != null && enemyTargetHealth != null && !enemyTargetHealth.IsDead() && !IsInRange(enemyTarget))
        {
            if (PlayerMovement.CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position,
                    Time.deltaTime * PlayerStats.MovementSpeed.GetRealMovementSpeed());

                PlayerMovement.NotifyPlayerMoved();
            }

            yield return null;
        }
        
        if (enemyTarget != null && enemyTargetHealth != null && !enemyTargetHealth.IsDead())
        {
            if(IsInRangeForSkill != null)
            {
                IsInRangeForSkill();
            }
            else if(IsInRangeForBasicAttack != null)
            {
                IsInRangeForBasicAttack(enemyTarget.gameObject.GetComponent<Player>().PlayerId);
            }
        }
        
        lastNetworkTarget = -1;
    }
}
