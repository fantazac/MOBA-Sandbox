using UnityEngine;
using System.Collections;

public class PlayerAttackMovement : PlayerBase
{
    private int lastNetworkTarget;

    public delegate void IsInRangeHandler();
    public event IsInRangeHandler IsInRange;

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

    public void ActivateMovementTowardsUnfriendlyTarget()
    {
        if (Player.nextAction == Actions.SKILL || Player.nextAction == Actions.MOVE)
        {
            Player.nextAction = Actions.ATTACK;
            StopAllCoroutines();
            PlayerOrientation.StopAllCoroutines();
        }
        else if (lastNetworkTarget != -1 && PlayerMovement.CanUseMovement())
        {
            PlayerNormalMovement.StopMovement();
            PhotonView.RPC("MoveTowardsUnfriendlyTargetFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkTarget);
        }
    }

    public bool WasMovingBeforeSkill()
    {
        return lastNetworkTarget != -1;
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        IsInRange = null;
        lastNetworkTarget = -1;
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
        IsInRange = null;
        PlayerNormalMovement.StopMovement();
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsUnfriendlyTarget(unfriendlyTarget, range));
        PlayerOrientation.RotatePlayerUntilReachedTarget(unfriendlyTarget);
    }

    public void SetMoveTowardsUnfriendlyTarget(Transform unfriendlyTarget)
    {
        SetMoveTowardsUnfriendlyTarget(unfriendlyTarget, Player.range);
    }
    
    private IEnumerator MoveTowardsUnfriendlyTarget(Transform enemyTarget, float range)
    {
        //can come in range for a screen and not for another, so the skill is done on one but not the other, use RPC
        while (enemyTarget != null && Vector3.Distance(transform.position, enemyTarget.position) > range)
        {
            if (PlayerMovement.CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position,
                    Time.deltaTime * Player.movementSpeed);

                PlayerMovement.NotifyPlayerMoved();
            }

            yield return null;
        }

        if (IsInRange != null && enemyTarget != null)
        {
            IsInRange();
        }
        
        lastNetworkTarget = -1;
    }
}
