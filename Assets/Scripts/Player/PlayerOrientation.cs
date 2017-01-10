﻿using UnityEngine;
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

    private bool CanRotate()
    {
        foreach (PlayerSkill ps in PlayerMovement.skills)
        {
            if (ps != null && !ps.canRotateWhileCasting && ps.skillActive)
            {
                return false;
            }
        }
        return true;
    }

    public void RotatePlayer(Vector3 clickedPosition)
    {
        StopAllCoroutines();
        StartCoroutine(Rotate(clickedPosition));
    }

    public void RotatePlayerInstantly(Vector3 clickedPosition)
    {
        transform.rotation = Quaternion.LookRotation((clickedPosition - transform.position).normalized);
    }

    private IEnumerator Rotate(Vector3 clickedPosition)
    {
        rotationAmount = Vector3.up;
        rotationAmountLastFrame = Vector3.zero;
        while (rotationAmountLastFrame != rotationAmount)
        {
            if (CanRotate())
            {
                rotationAmountLastFrame = rotationAmount;

                rotationAmount = Vector3.RotateTowards(transform.forward, clickedPosition - transform.position, Time.deltaTime * rotationSpeed, 0);

                transform.rotation = Quaternion.LookRotation(rotationAmount);
            }

            yield return null;
        }
    }

}
