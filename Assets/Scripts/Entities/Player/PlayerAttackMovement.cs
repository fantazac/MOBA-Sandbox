using UnityEngine;
using System.Collections;

public class PlayerAttackMovement : PlayerBase
{
    private int lastNetworkTarget;

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
        lastNetworkTarget = -1;
    }

    [PunRPC]
    private void MoveTowardsUnfriendlyTargetFromServer(int enemyPlayerId)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        SetMoveTowardsUnfriendlyTarget(FindEnemyPlayer(enemyPlayerId));
    }

    private Transform FindEnemyPlayer(int enemyPlayerId)
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

        return enemyPlayer.transform;
    }

    private void SetMoveTowardsUnfriendlyTarget(Transform unfriendlyTarget)
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsUnfriendlyTarget(unfriendlyTarget));
        PlayerOrientation.RotatePlayerUntilReachedTarget(unfriendlyTarget);
    }
    
    private IEnumerator MoveTowardsUnfriendlyTarget(Transform enemyTarget)
    {
        while (enemyTarget != null && Vector3.Distance(transform.position, enemyTarget.position) > (Player.range / 100f))
        {
            if (PlayerMovement.CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position,
                    Time.deltaTime * (Player.movementSpeed / 100f));

                PlayerMovement.NotifyPlayerMoved();
            }

            yield return null;
        }
        lastNetworkTarget = -1;
    }
}
