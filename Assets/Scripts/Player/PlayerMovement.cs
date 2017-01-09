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

    private GameObject capsule;

    [HideInInspector]
    public Vector3 halfHeight;
    [HideInInspector]
    public Vector3 halfWidth;

    public Vector3 targetCapsulePosition;
    public Vector3 wherePlayerClicked;

    private Vector3 networkMove;

    private InputManager inputManager;
    public List<PlayerSkill> skills;
    private bool isDashing = false;
    [HideInInspector]
    public bool isShootingProjectile = false;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.OnRightClick += PressedRightClick;
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        childCamera = transform.parent.GetComponentInChildren<Camera>();
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;
        halfWidth = Vector3.forward * transform.localScale.z * 0.5f;

        foreach (PlayerSkill ps in GetComponents<PlayerSkill>())
        {
            ps.SkillActivated += SkillActivated;
            ps.SkillFinished += SkillFinished;
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
            if (!isDashing)
            {
                StopAllCoroutines();
                StartCoroutine(Move(targetCapsulePosition));
                PlayerOrientation.RotatePlayer(targetCapsulePosition);
            }
        }
    }

    public Ray GetRay(Vector3 mousePosition)
    {
        return childCamera.ScreenPointToRay(mousePosition);
    }

    private void SkillActivated(int skillId, Vector3 mousePositionOnTerrain)
    {
        switch (skillId)
        {
            case 0:
                ShootingProjectile(mousePositionOnTerrain);
                break;
            case 1:
                Dashing();
                break;
        }
    }

    private void SkillFinished(int skillId)
    {
        switch (skillId)
        {
            case 0:
                DoneShootingProjectile();
                break;
            case 1:
                DashingFinished();
                break;
        }
    }

    private void Dashing()
    {
        isDashing = true;
        StopAllCoroutines();
    }

    private void DashingFinished()
    {
        isDashing = false;
        if(targetCapsulePosition != Vector3.zero && !isShootingProjectile)
        {
            StopAllCoroutines();
            StartCoroutine(Move(targetCapsulePosition));
            PlayerOrientation.RotatePlayer(targetCapsulePosition);
        }
    }

    private void ShootingProjectile(Vector3 mousePositionOnTerrain)
    {
        isShootingProjectile = true;
        PlayerOrientation.RotatePlayerInstantly(mousePositionOnTerrain);
    }

    private void DoneShootingProjectile()
    {
        isShootingProjectile = false;
        if (targetCapsulePosition != Vector3.zero)
        {
            StopAllCoroutines();
            StartCoroutine(Move(targetCapsulePosition));
            PlayerOrientation.RotatePlayer(targetCapsulePosition);
        }
    }

    public void PlayerDashing()
    {
        PlayerMoved();
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

            if (!PhotonView.isMine && networkMove != wherePlayerClicked)
            {
                StopAllCoroutines();
                StartCoroutine(Move(networkMove));
                PlayerOrientation.RotatePlayer(networkMove);
            }
        }
    }

    private IEnumerator Move(Vector3 wherePlayerClickedToMove)
    {
        wherePlayerClicked = wherePlayerClickedToMove;
        while (transform.position != wherePlayerClickedToMove)
        {
            if (!isDashing && !isShootingProjectile)
            {
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * 10);

                if(PlayerMoved != null)
                {
                    PlayerMoved();
                }
            }

            yield return null;
        }
        targetCapsulePosition = Vector3.zero;
        Destroy(capsule);
    }
}

/*using UnityEngine;
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

    public Vector3 wherePlayerClicked;
    public Vector3 networkClicked;

    private Vector3 networkMove;

    private InputManager inputManager;
    public List<PlayerSkill> skills;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    protected override void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.OnRightClick += PressedRightClick;
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        childCamera = transform.parent.GetComponentInChildren<Camera>();
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;
        wherePlayerClicked = transform.position;

        foreach (PlayerSkill ps in GetComponents<PlayerSkill>())
        {
            ps.SkillActivated += SkillActivated;
            ps.SkillFinished += SkillFinished;
        }

        base.Start();
    }

    private void PressedRightClick(Vector3 mousePosition)
    {
        RaycastHit hit;
        if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            //Destroy(capsule);
            //capsule = (GameObject)Instantiate(moveToCapsule, hit.point + halfHeight, new Quaternion());
            //targetCapsulePosition = hit.point + halfHeight;
            PhotonView.RPC("MovePlayerServer", PhotonTargets.AllViaServer, hit.point + halfHeight);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(wherePlayerClicked.ToString());
    }

    [PunRPC]
    private void MovePlayerServer(Vector3 clickedPoint)
    {
        if (PhotonView.isMine)
        {
            Destroy(capsule);
            capsule = (GameObject)Instantiate(moveToCapsule, clickedPoint, new Quaternion());
            wherePlayerClicked = clickedPoint;
        }
        else
        {
            networkClicked = clickedPoint;
        }
    }

    public Ray GetRay(Vector3 mousePosition)
    {
        return childCamera.ScreenPointToRay(mousePosition);
    }

    public override void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(wherePlayerClicked);
        }
        else
        {
            wherePlayerClicked = (Vector3)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (PhotonView.isMine)
        {
            UpdatePosition();
        }
        else
        {
            UpdateNetworkPosition();
        }
    }

    private void UpdatePosition()
    {
        if (Vector3.Distance(transform.position, wherePlayerClicked) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, wherePlayerClicked, Time.deltaTime * 10);
        }
        else if (PhotonView.isMine && capsule != null)
        {
            Destroy(capsule);
        }
    }

    private void UpdateNetworkPosition()
    {
        Debug.Log(transform.position);
        transform.position = Vector3.MoveTowards(transform.position, networkClicked, Time.deltaTime * 10);
    }
}*/

