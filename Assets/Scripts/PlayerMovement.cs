using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject moveTo;
    [HideInInspector]
    public TerrainCollider terrainCollider;
    private Camera childCamera;

    private GameObject moveToPoint;

    [HideInInspector]
    public Vector3 halfHeight;
    [HideInInspector]
    public Vector3 halfWidth;

    private Vector3 rotationAmountLastFrame;
    private Vector3 rotationAmount;

    private Vector3 target;

    private InputManager inputManager;
    public List<PlayerSkill> skills;
    private bool isDashing = false;
    [HideInInspector]
    public bool isShootingProjectile = false;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    private void Start()
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
    }

    private void PressedRightClick(Vector3 mousePosition)
    {
        RaycastHit hit;
        if (terrainCollider.Raycast(GetRay(mousePosition), out hit, Mathf.Infinity))
        {
            Destroy(moveToPoint);
            moveToPoint = (GameObject)Instantiate(moveTo, hit.point + halfHeight, new Quaternion());
            target = hit.point + halfHeight;
            if (!isDashing)
            {
                StopAllCoroutines();
                StartCoroutine(MoveTowardsWherePlayerClicked(target));
                StartCoroutine(RotateTowardsWherePlayerClicked(target));
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
        if(target != Vector3.zero && !isShootingProjectile)
        {
            StopAllCoroutines();
            StartCoroutine(MoveTowardsWherePlayerClicked(target));
            StartCoroutine(RotateTowardsWherePlayerClicked(target));
        }
    }

    private void ShootingProjectile(Vector3 mousePositionOnTerrain)
    {
        isShootingProjectile = true;
        RotateTowardsTargetInstantly(mousePositionOnTerrain);
    }

    private void DoneShootingProjectile()
    {
        isShootingProjectile = false;
        if (target != Vector3.zero)
        {
            StopAllCoroutines();
            StartCoroutine(MoveTowardsWherePlayerClicked(target));
            StartCoroutine(RotateTowardsWherePlayerClicked(target));
        }
    }

    public void PlayerDashing()
    {
        PlayerMoved();
    }

    private IEnumerator MoveTowardsWherePlayerClicked(Vector3 wherePlayerClickedToMove)
    {
        while(transform.position != wherePlayerClickedToMove)
        {
            if (!isDashing && !isShootingProjectile)
            {
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * 10);

                PlayerMoved();
            }

            yield return null;
        }
        target = Vector3.zero;
        Destroy(moveToPoint);
    }

    private void RotateTowardsTargetInstantly(Vector3 wherePlayerClickedToMove)
    {
        rotationAmount = (wherePlayerClickedToMove - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(rotationAmount);
    }

    private IEnumerator RotateTowardsWherePlayerClicked(Vector3 wherePlayerClickedToMove)
    {
        rotationAmount = Vector3.up;
        rotationAmountLastFrame = Vector3.zero;
        while (rotationAmountLastFrame != rotationAmount)
        {
            if (!isDashing)
            {
                rotationAmountLastFrame = rotationAmount;

                rotationAmount = Vector3.RotateTowards(transform.forward, wherePlayerClickedToMove - transform.position, Time.deltaTime * 15, 0);

                transform.rotation = Quaternion.LookRotation(rotationAmount);
            }

            yield return null;
        }
    }
}
