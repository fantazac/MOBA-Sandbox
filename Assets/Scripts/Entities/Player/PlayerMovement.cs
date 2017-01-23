using UnityEngine;
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

    private RaycastHit hit;

    public Vector3 lastNetworkMove;
    public int lastNetworkTarget;

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
            }
        }

        base.Start();
    }

    private void PressedRightClick(Vector3 mousePosition)
    {
        if (PlayerMouseSelection.HoveredObjectIsEnemy(EntityTeam.Team))
        {
            lastNetworkTarget = PlayerMouseSelection.HoveredObject.GetComponent<Player>().PlayerId;
            ActivateMovementTowardsEnemyPlayer();
        }
        else if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            Instantiate(moveToCapsule, hit.point, new Quaternion());
            lastNetworkMove = hit.point + halfHeight;
            ActivateMovementTowardsPoint();
        }
    }

    private void StopMovement()
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
    }

    private bool CanUseMovement()
    {
        foreach(PlayerSkill ps in Player.skills)
        {
            if(ps != null && ps.skillIsActive && !ps.canMoveWhileCasting)
            {
                return false;
            }
        }
        return true;
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

    private Transform FindEnemyPlayer(int enemyPlayerId)
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
        
        return enemyPlayer.transform;
    }

    public void CancelMovement()
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
        lastNetworkMove = Vector3.zero;
        lastNetworkTarget = -1;
    }

    public void ActivateMovementTowardsEnemyPlayer()
    {
        if(Player.nextAction == "UseSkillFromServer" || Player.nextAction == "Move")
        {
            Player.nextAction = "Attack";
        }
        else if(lastNetworkTarget != -1 && CanUseMovement())
        {
            lastNetworkMove = Vector3.zero;
            PhotonView.RPC("MoveTowardsEnemyPlayerFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkTarget);
        }
    }

    public void ActivateMovementTowardsPoint()
    {
        if (Player.nextAction == "UseSkillFromServer" || Player.nextAction == "Attack")
        {
            Player.nextAction = "Move";
        }
        else if(lastNetworkMove != Vector3.zero && CanUseMovement())
        {
            lastNetworkTarget = -1;
            PhotonView.RPC("MoveTowardsPointFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkMove);
        }
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

    private void SetMoveTowardsObject(Transform enemyTarget)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowardsObject(enemyTarget));
        PlayerOrientation.RotatePlayerUntilReachedTarget(enemyTarget);
    }

    private void SetMoveTowardsPoint(Vector3 wherePlayerClickedToMove)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTowardsPoint(wherePlayerClickedToMove));
        PlayerOrientation.RotatePlayer(wherePlayerClickedToMove);
    }

    private IEnumerator MoveTowardsObject(Transform enemyTarget)
    {
        while (enemyTarget != null)
        {
            if (CanUseMovement())
            {
                transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position, 
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
    }
}
