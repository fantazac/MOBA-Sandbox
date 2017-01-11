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

    private GameObject capsule;

    [HideInInspector]
    public Vector3 halfHeight;
    [HideInInspector]
    public Vector3 halfWidth;

    public Vector3 targetCapsulePosition;
    public Vector3 wherePlayerClicked;

    private Vector3 networkMove;
    private Vector3 lastNetworkMove;
    private float lastNetworkDataReceivedTime;
    public float totalTimePassed;
    private float pingInSeconds;
    private float timeSinceLastUpdate;
    private WaitForSeconds delayPing;

    public List<PlayerSkill> skills;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        PlayerInput.OnRightClick += PressedRightClick;
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        if (PhotonView.isMine)
        {
            childCamera = transform.parent.GetComponentInChildren<Camera>();
        }
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;
        halfWidth = Vector3.forward * transform.localScale.z * 0.5f;

        foreach (PlayerSkill ps in skills)
        {
            if(ps != null)
            {
                if (ps.continueMovementAfterCast)
                {
                    ps.SkillFinished += ContinueMovementAfterSkill;
                }
                else
                {
                    ps.SkillStarted += StopMovementOnSkillCast;
                }
            }
        }

        base.Start();
    }

    private void PressedRightClick(Vector3 mousePosition)
    {
        RaycastHit hit;
        if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            Destroy(capsule);
            capsule = (GameObject)Instantiate(moveToCapsule, hit.point + halfHeight, new Quaternion());
            targetCapsulePosition = hit.point + halfHeight;
            if (CanUseMovement())
            {
                delayPing = new WaitForSeconds((float)PhotonNetwork.GetPing() * 0.001f);
                StartCoroutine(MoveDelay(targetCapsulePosition));
            }
        }
    }

    private bool CanUseMovement()
    {
        foreach(PlayerSkill ps in skills)
        {
            if(ps != null && !ps.canMoveWhileCasting && ps.skillActive)
            {
                return false;
            }
        }
        return true;
    }

    public bool CanCastSpell(PlayerSkill skill)
    {
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null && ps.skillActive && !ps.castableSpellsWhileActive.Contains(skill))
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
            StartCoroutine(MakeCapsuleDisapear());
            StartCoroutine(Move(PhotonView.isMine ? targetCapsulePosition : lastNetworkMove));
            PlayerOrientation.RotatePlayer(PhotonView.isMine ? targetCapsulePosition : lastNetworkMove);
        }
    }

    private void StopMovementOnSkillCast()
    {
        StopAllCoroutines();
        StartCoroutine(MakeCapsuleDisapear());
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

    public override void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(wherePlayerClicked);
        }
        else
        {
            networkMove = (Vector3)stream.ReceiveNext();

            if (lastNetworkMove != networkMove && !PhotonView.isMine)
            {
                lastNetworkMove = networkMove;

                /*pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
                //timeSinceLastUpdate = (float)(PhotonNetwork.time - lastNetworkDataReceivedTime);
                lastNetworkDataReceivedTime = (float)PhotonNetwork.time;
                totalTimePassed = pingInSeconds; //+ timeSinceLastUpdate;*/

                StopAllCoroutines();
                //StartCoroutine(MakeCapsuleDisapear());
                StartCoroutine(Move(networkMove));
                PlayerOrientation.RotatePlayer(networkMove);
            }
        }
    }

    private IEnumerator MoveDelay(Vector3 wherePlayerClickedToMove)
    {
        yield return delayPing;

        SetMove(wherePlayerClickedToMove);
    }

    private void SetMove(Vector3 wherePlayerClickedToMove)
    {
        StopAllCoroutines();
        StartCoroutine(MakeCapsuleDisapear());
        StartCoroutine(Move(targetCapsulePosition));
        PlayerOrientation.RotatePlayer(targetCapsulePosition);
    }

    private IEnumerator Move(Vector3 wherePlayerClickedToMove)
    {
        wherePlayerClicked = wherePlayerClickedToMove;

        while (transform.position != wherePlayerClickedToMove)
        {
            if (CanUseMovement())
            {
                /*if (totalTimePassed != 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, totalTimePassed * 10);
                    totalTimePassed = 0;
                }*/
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * 10);

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

    private IEnumerator MakeCapsuleDisapear()
    {
        if(capsule != null)
        {
            while (capsule.transform.position.y > -1)
            {
                capsule.transform.position += Vector3.down * 0.06f;

                yield return null;
            }

            Destroy(capsule);
        }
    }

    private void OnDestroy()
    {
        Destroy(capsule);
    }
}
