using UnityEngine;
using System.Collections;

public class PlayerNormalMovement : PlayerBase
{
    [SerializeField]
    private GameObject moveToCapsule;

    private Vector3 lastNetworkMove;

    protected override void Start()
    {
        lastNetworkMove = Vector3.zero;
        base.Start();
    }

    public void UseNormalMovement(Vector3 mousePosition)
    {
        lastNetworkMove = mousePosition;
        Instantiate(moveToCapsule, mousePosition, new Quaternion());
        ActivateMovementTowardsPoint();
    }

    public void ActivateMovementTowardsPoint()
    {
        if (Player.nextAction == Actions.SKILL || Player.nextAction == Actions.ATTACK)
        {
            Player.nextAction = Actions.MOVE;
            StopAllCoroutines();
            PlayerOrientation.StopAllCoroutines();
        }
        else if (lastNetworkMove != Vector3.zero)
        {
            if (PlayerMovement.CanUseMovement())
            {
                PlayerAttackMovement.StopMovement();
                PhotonView.RPC("MoveTowardsPointFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkMove);
            }
            else
            {
                Player.nextAction = Actions.MOVE;
            }
        }
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        lastNetworkMove = Vector3.zero;
    }

    public bool WasMovingBeforeSkill()
    {
        return lastNetworkMove != Vector3.zero;
    }

    [PunRPC]
    private void MoveTowardsPointFromServer(Vector3 wherePlayerClicked)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        SetMoveTowardsPoint(wherePlayerClicked);
    }

    private void SetMoveTowardsPoint(Vector3 wherePlayerClickedToMove)
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsPoint(wherePlayerClickedToMove));
        PlayerOrientation.RotatePlayer(wherePlayerClickedToMove);
    }

    private IEnumerator MoveTowardsPoint(Vector3 wherePlayerClickedToMove)
    {
        while (transform.position != wherePlayerClickedToMove)
        {
            if (PlayerMovement.CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * (Player.movementSpeed / 100f));

                PlayerMovement.NotifyPlayerMoved();
            }

            yield return null;
        }
        lastNetworkMove = Vector3.zero;
    }
}
