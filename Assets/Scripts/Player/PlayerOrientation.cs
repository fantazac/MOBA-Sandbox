using UnityEngine;
using System.Collections;

public class PlayerOrientation : PlayerBase
{
    private Vector3 networkMove;

    [SerializeField]
    private int rotationSpeed = 15;

    private Vector3 rotationAmountLastFrame;
    private Vector3 rotationAmount;

    protected override void Start()
    {
        base.Start();
    }

    public override void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(PlayerMovement.wherePlayerClicked);
        }
        else
        {
            networkMove = (Vector3)stream.ReceiveNext();

            if (!PhotonView.isMine && networkMove != PlayerMovement.wherePlayerClicked)
            {
                StopAllCoroutines();
                StartCoroutine(Rotate(networkMove));
            }
        }
    }

    public void RotatePlayer(Vector3 clickedPosition)
    {
        StopAllCoroutines();
        StartCoroutine(Rotate(clickedPosition));
    }

    public void RotatePlayerInstantly(Vector3 clickedPosition)
    {
        rotationAmount = (clickedPosition - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(rotationAmount);
    }

    private IEnumerator Rotate(Vector3 clickedPosition)
    {
        rotationAmount = Vector3.up;
        rotationAmountLastFrame = Vector3.zero;
        while (rotationAmountLastFrame != rotationAmount)
        {
            rotationAmountLastFrame = rotationAmount;

            rotationAmount = Vector3.RotateTowards(transform.forward, clickedPosition - transform.position, Time.deltaTime * rotationSpeed, 0);

            transform.rotation = Quaternion.LookRotation(rotationAmount);

            yield return null;
        }
    }

}
