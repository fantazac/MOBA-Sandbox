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

    //Make it so after casting a spell, you have 3 options:
    //1. continue where you were heading/stay immobile cause you didnt click before casting
    //2. click to move while casting -> go there after skill done
    //3. do another spell while casting another, which will go through when the other is done

    //Basically, riot stores next action "somewhere" if there's an input during cast time, whether skill or movement

    //pressing s "empties" the action line, aka cancels either next skillcast or movement
    //there are 2 "events": currentlyCasting and nextCast. can be a skill that has a cast time or a move/basic attack

    //LUCIAN
    //Q: if moving --> q, stops movement. if moving --> q + rightclick, move after spell, cannot buffer e with q (its cancelled/gray)
    //W: if moving --> w, move after spell. W and R have same interaction with e, can cast E while cast time/cast other 2 while Eing
    //E: does not count as a skill, aka no cast time. if e while culling and not rightclick during e, stop moving
    //R: counts as a skill even if has no cast time.
    

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
        if (PlayerMouseSelection.HoveredObjectIsEnemy(EntityTeam.Team))
        {
            if (CanUseMovement())
            {
                ActivateMovementTowardsEnemyPlayer();
            }
        }
        else if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            Instantiate(moveToCapsule, hit.point, new Quaternion());
            targetCapsulePosition = hit.point + halfHeight;
            if (CanUseMovement())   //check if in league you can move after doing: 
            {                       //move -> ezreal q (stops movement) -> click while casting -> move?
                ActivateMovementTowardsPoint(); //if yes, change how this works
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
            StartCoroutine(MoveTowardsPoint(PhotonView.isMine ? hit.point + halfHeight : lastNetworkMove));
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

    private GameObject FindEnemyPlayer(int enemyPlayerId)
    {
        GameObject enemyPlayer = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if(player.GetComponent<Player>().PlayerId == enemyPlayerId)
            {
                enemyPlayer = player;
                break;
            }
        }
        
        return enemyPlayer;
    }

    private void ActivateMovementTowardsEnemyPlayer()
    {
        PhotonView.RPC("MoveTowardsEnemyPlayerFromServer", PhotonTargets.AllBufferedViaServer, 
            PlayerMouseSelection.HoveredObject.GetComponent<Player>().PlayerId);
    }

    private void ActivateMovementTowardsPoint()
    {
        PhotonView.RPC("MoveTowardsPointFromServer", PhotonTargets.AllBufferedViaServer, hit.point + halfHeight);
    }

    [PunRPC]
    private void MoveTowardsEnemyPlayerFromServer(int enemyPlayerId)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        SetMoveTowardsObject(FindEnemyPlayer(enemyPlayerId));
    }

    [PunRPC]
    private void MoveTowardsPointFromServer(Vector3 wherePlayerClicked)
    {
        //If a traget is moving and you connect, this is called, which works as intented.
        //But, the target will start moving from its spawn instead of "where it's supposed to be at the current time"
        //Fix this
        SetMoveTowardsPoint(wherePlayerClicked);
    }

    private void SetMoveTowardsObject(GameObject enemyTarget)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowardsObject(enemyTarget));
        PlayerOrientation.RotatePlayerUntilReachedTarget(enemyTarget);
    }

    private void SetMoveTowardsPoint(Vector3 wherePlayerClickedToMove)
    {
        lastNetworkMove = wherePlayerClickedToMove;
        StopAllCoroutines();
        StartCoroutine(MoveTowardsPoint(wherePlayerClickedToMove));
        PlayerOrientation.RotatePlayer(wherePlayerClickedToMove);
    }

    private IEnumerator MoveTowardsObject(GameObject enemyTarget)
    {
        while (enemyTarget != null)
        {
            if (CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.transform.position, 
                    Time.deltaTime * (Player.movementSpeed / 100f));

                if (PlayerMoved != null)
                {
                    PlayerMoved();
                }
            }

            yield return null;
        }
    }

    private IEnumerator MoveTowardsPoint(Vector3 wherePlayerClickedToMove)
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
