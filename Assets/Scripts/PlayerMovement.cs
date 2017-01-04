using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject moveTo;

    private Rigidbody _rigidbody;
    public TerrainCollider terrainCollider;
    private Camera childCamera;

    private GameObject moveToPoint;

    public Vector3 halfHeight;

    private Vector3 rotationAmountLastFrame;
    private Vector3 rotationAmount;

    private Vector3 target;

    private LucianE lucianE;
    private bool isUsingLucianE = false;

    private bool isMoving = false;

    public delegate void PlayerMovedHandler();
    public event PlayerMovedHandler PlayerMoved;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        childCamera = transform.parent.GetComponentInChildren<Camera>();
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;
        lucianE = GetComponent<LucianE>();
        lucianE.LucianEActivated += LucianEActivated;
        lucianE.LucianEFinished += LucianEFinished;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (terrainCollider.Raycast(GetRay(), out hit, Mathf.Infinity))
            {
                Destroy(moveToPoint);
                moveToPoint = (GameObject)Instantiate(moveTo, hit.point + halfHeight, new Quaternion());
                target = hit.point + halfHeight;
                if (!isUsingLucianE)
                {
                    StopAllCoroutines();
                    StartCoroutine(MoveTowardsWherePlayerClicked(target));
                    StartCoroutine(RotateTowardsWherePlayerClicked(target));
                }
            }
        }
    }

    public Ray GetRay()
    {
        return childCamera.ScreenPointToRay(Input.mousePosition);
    }

    private void LucianEActivated()
    {
        isUsingLucianE = true;
        StopAllCoroutines();
    }

    private void LucianEFinished()
    {
        isUsingLucianE = false;
        if(target != Vector3.zero)
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
            if (!isUsingLucianE)
            {
                transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * 10);

                PlayerMoved();
            }

            yield return null;
        }
        target = Vector3.zero;
        Destroy(moveToPoint);
    }

    private IEnumerator RotateTowardsWherePlayerClicked(Vector3 wherePlayerClickedToMove)
    {
        rotationAmount = Vector3.up;
        while (rotationAmountLastFrame != rotationAmount)
        {
            if (!isUsingLucianE)
            {
                rotationAmountLastFrame = rotationAmount;

                rotationAmount = Vector3.RotateTowards(transform.forward, wherePlayerClickedToMove - transform.position, Time.deltaTime * 15, 0);

                transform.rotation = Quaternion.LookRotation(rotationAmount);
            }

            yield return null;
        }
    }
}
