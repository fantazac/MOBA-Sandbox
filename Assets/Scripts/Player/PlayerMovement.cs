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

    [HideInInspector]
    public Vector3 targetCapsulePosition;
    [HideInInspector]
    public Vector3 wherePlayerClicked;

    private Vector3 networkMove;
    private Vector3 lastNetworkMove;
    private WaitForSeconds delayPing;

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
        halfWidth = Vector3.forward * transform.localScale.z * 0.5f;

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
            Destroy(capsule);
            capsule = (GameObject)Instantiate(moveToCapsule, hit.point + halfHeight, new Quaternion());
            targetCapsulePosition = hit.point + halfHeight;
            if (CanUseMovement())
            {
                wherePlayerClicked = hit.point + halfHeight;
                delayPing = new WaitForSeconds((float)PhotonNetwork.GetPing() * 0.001f);
                StartCoroutine(MoveDelay(hit.point + halfHeight));
            }
        }
    }

    private void StopMovement()
    {
        targetCapsulePosition = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(MakeCapsuleDisapear());
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
            StartCoroutine(Move(PhotonView.isMine ? hit.point + halfHeight : lastNetworkMove));
            PlayerOrientation.RotatePlayer(PhotonView.isMine ? hit.point + halfHeight : lastNetworkMove);
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

                StopAllCoroutines();
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
