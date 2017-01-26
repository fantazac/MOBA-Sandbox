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

    [HideInInspector]
    public Vector3 lastNetworkMove;
    [HideInInspector]
    public int lastNetworkTarget;

    [HideInInspector]
    public Vector3 spawnPoint;

    [HideInInspector]
    public Health playerHealth;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        PlayerInput.OnRightClick += PressedRightClick;
        PlayerInput.OnPressedS += StopMovement;
        PlayerInput.OnPressedM += TeleportMid;

        playerHealth = GetComponent<Health>();
        playerHealth.OnDeath += OnDeath;

        GetComponent<PlayerDeath>().RespawnPlayer += RespawnPlayerInBase;

        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        if (PhotonView.isMine)
        {
            childCamera = transform.parent.GetComponentInChildren<Camera>();
        }
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;

        for (int i = 0; i < Player.skills.Count; i++)
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
        if (!playerHealth.IsDead())
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
    }

    private void TeleportMid()
    {
        transform.position = halfHeight;
        if(PlayerMoved != null)
        {
            PlayerMoved();
        }
        StopMovement();
    }

    private void OnDeath()
    {
        Player.healthBar.SetActive(false);
        //stop all skills (cancel cast time of current skill, stop skills with no cast time), 
        //then stop movement, then start death animation
        StopMovement();
        DeathAnimation();
    }

    private void RespawnPlayerInBase()
    {
        Player.healthBar.SetActive(true);
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;
        if (PlayerMoved != null)
        {
            PlayerMoved();
        }
    }

    private void DeathAnimation()
    {
        StartCoroutine(SinkThroughFloorOnDeath());
    }

    private IEnumerator SinkThroughFloorOnDeath()
    {
        while(transform.position.y > -1)
        {
            transform.position = transform.position - Vector3.up * 0.06f;

            yield return null;
        }
    }

    private void StopMovement()
    {
        CancelMovement();
        Player.nextAction = "";
    }

    private bool CanUseMovement()
    {
        foreach (PlayerSkill ps in Player.skills)
        {
            if (ps != null && ps.skillIsActive && !ps.canMoveWhileCasting)
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
        if (PlayerMoved != null)
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
            if (player.GetComponent<Player>().PlayerId == enemyPlayerId)
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
        if (Player.nextAction == "UseSkillFromServer" || Player.nextAction == "Move")
        {
            Player.nextAction = "Attack";
            StopAllCoroutines();
            PlayerOrientation.StopAllCoroutines();
        }
        else if (lastNetworkTarget != -1 && CanUseMovement())
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
            StopAllCoroutines();
            PlayerOrientation.StopAllCoroutines();
        }
        else if (lastNetworkMove != Vector3.zero)
        {
            if (CanUseMovement())
            {
                lastNetworkTarget = -1;
                PhotonView.RPC("MoveTowardsPointFromServer", PhotonTargets.AllBufferedViaServer, lastNetworkMove);
            }
            else
            {
                Player.nextAction = "Move";
            }
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
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsObject(enemyTarget));
        PlayerOrientation.RotatePlayerUntilReachedTarget(enemyTarget);
    }

    private void SetMoveTowardsPoint(Vector3 wherePlayerClickedToMove)
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
        StartCoroutine(MoveTowardsPoint(wherePlayerClickedToMove));
        PlayerOrientation.RotatePlayer(wherePlayerClickedToMove);
    }

    private IEnumerator MoveTowardsObject(Transform enemyTarget)
    {
        while (enemyTarget != null && Vector3.Distance(transform.position, enemyTarget.position) > (Player.range / 100f))
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
        lastNetworkTarget = -1;
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
