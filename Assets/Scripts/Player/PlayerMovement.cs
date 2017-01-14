﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : PlayerBase
{
    [SerializeField]
    private GameObject moveToCapsule;

    [HideInInspector]
    public TerrainCollider terrainCollider;
    private Camera childCamera;

    [HideInInspector]
    public Vector3 halfHeight;

    private Vector3 targetCapsulePosition;

    private Vector3 lastNetworkMove;

    private RaycastHit hit;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        PlayerInput.OnRightClick += PressedRightClick;
        PlayerInput.OnPressedS += StopMovement;
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        if (PhotonView.isMine)
        {
            childCamera = transform.parent.GetComponentInChildren<Camera>();
        }
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;

        for(int i = 0; i < Player.skills.Count; i++)
        {
            if (Player.skills[i] != null)
            {
                Player.skills[i].SetSkillId(i);
                if (Player.skills[i].continueMovementAfterCast)
                {
                    Player.skills[i].SkillFinished += ContinueMovementAfterSkill;
                }
                else
                {
                    Player.skills[i].SkillStarted += StopMovementOnSkillCast;
                }
            }
        }

        base.Start();
    }

    private void PressedRightClick(Vector3 mousePosition)
    {
        if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            Instantiate(moveToCapsule, hit.point, new Quaternion());
            targetCapsulePosition = hit.point + halfHeight;
            if (CanUseMovement())   //check if in league you can move after doing: 
            {                       //move -> ezreal q (stops movement) -> click while casting -> move?
                ActivateMovement(); //if yes, change how this works
            }
        }
    }

    private void StopMovement()
    {
        targetCapsulePosition = Vector3.zero;
        StopAllCoroutines();
    }

    private bool CanUseMovement()
    {
        foreach(PlayerSkill ps in Player.skills)
        {
            if(ps != null && !ps.canMoveWhileCasting && ps.skillActive)
            {
                return false;
            }
        }
        return true;
    }

    private void ContinueMovementAfterSkill()
    {
        if (((!PhotonView.isMine && lastNetworkMove != Vector3.zero) || targetCapsulePosition != Vector3.zero) && CanUseMovement())
        {
            StopAllCoroutines();
            StartCoroutine(Move(PhotonView.isMine ? hit.point + halfHeight : lastNetworkMove));
            PlayerOrientation.RotatePlayer(PhotonView.isMine ? hit.point + halfHeight : lastNetworkMove);
        }
    }

    private void StopMovementOnSkillCast()
    {
        StopAllCoroutines();
    }

    public Ray GetRay(Vector3 mousePosition)
    {
        return childCamera.ScreenPointToRay(mousePosition);
    }

    public void PlayerMovingWithSkill()
    {
        if(PlayerMoved != null)
        {
            PlayerMoved();
        }
    }

    private void ActivateMovement()
    {
        PhotonView.RPC("MoveFromServer", PhotonTargets.AllBufferedViaServer, hit.point + halfHeight);
    }

    [PunRPC]
    private void MoveFromServer(Vector3 wherePlayerClicked)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        SetMove(wherePlayerClicked);
    }

    private void SetMove(Vector3 wherePlayerClickedToMove)
    {
        lastNetworkMove = wherePlayerClickedToMove;
        StopAllCoroutines();
        StartCoroutine(Move(wherePlayerClickedToMove));
        PlayerOrientation.RotatePlayer(wherePlayerClickedToMove);
    }

    private IEnumerator Move(Vector3 wherePlayerClickedToMove)
    {
        while (transform.position != wherePlayerClickedToMove)
        {
            if (CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * (Player.movementSpeed / 100f));

                if (PlayerMoved != null)
                {
                    PlayerMoved();
                }
            }

            yield return null;
        }
        lastNetworkMove = Vector3.zero;
        targetCapsulePosition = Vector3.zero;
    }
}
