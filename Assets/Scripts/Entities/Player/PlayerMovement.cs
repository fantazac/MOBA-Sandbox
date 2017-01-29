using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : PlayerBase
{
    [HideInInspector]
    public TerrainCollider terrainCollider;

    private Camera childCamera;

    private RaycastHit hit;

    [HideInInspector]
    public Vector3 spawnPoint;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        PlayerInput.OnRightClick += PressedRightClick;
        PlayerInput.OnPressedS += StopMovementOnServer;
        PlayerInput.OnPressedM += TeleportMid;

        GetComponent<PlayerDeath>().RespawnPlayer += RespawnPlayerInBase;

        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        if (PhotonView.isMine)
        {
            childCamera = transform.parent.GetComponentInChildren<Camera>();
        }

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
        if (!Player.health.IsDead())
        {
            if (PlayerMouseSelection.HoveredObjectIsEnemy(EntityTeam.Team))
            {
                PlayerAttackMovement.UseAttackMove();
            }
            else if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
            {
                PlayerNormalMovement.UseNormalMovement(hit.point + Player.halfHeight);
            }
        }
    }

    private void TeleportMid()
    {
        PhotonView.RPC("TeleportMidFromServer", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void TeleportMidFromServer()
    {
        transform.position = Player.halfHeight;
        if (PlayerMoved != null)
        {
            PlayerMoved();
        }
        StopMovement();
    }

    private void RespawnPlayerInBase()
    {
        PhotonView.RPC("RespawnPlayerInBaseFromServer", PhotonTargets.AllBufferedViaServer, spawnPoint);
    }

    [PunRPC]
    private void RespawnPlayerInBaseFromServer(Vector3 spawn)
    {
        Player.healthBar.SetActive(true);
        transform.position = spawn;
        transform.rotation = Quaternion.identity;
        if (PlayerMoved != null)
        {
            PlayerMoved();
        }
    }

    private void StopMovementOnServer()
    {
        PhotonView.RPC("StopMovement", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void StopMovement()
    {
        CancelMovement();
        Player.nextAction = Actions.NONE;
    }

    public bool CanUseMovement()
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

    public void NotifyPlayerMoved()
    {
        if (PlayerMoved != null)
        {
            PlayerMoved();
        }
    }

    public void CancelMovement()
    {
        StopAllCoroutines();
        PlayerOrientation.StopAllCoroutines();
        PlayerNormalMovement.StopMovement();
        PlayerAttackMovement.StopMovement();
    }
    
}
