using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject moveTo;

    private Rigidbody _rigidbody;
    private TerrainCollider terrainCollider;
    private Camera childCamera;

    private GameObject moveToPoint;

    private Vector3 halfHeight;

    private bool isMoving = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        terrainCollider = GameObject.Find("Terrain").GetComponent<TerrainCollider>();
        childCamera = GetComponentInChildren<Camera>();
        halfHeight = Vector3.up * transform.localScale.y * 0.5f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = childCamera.ScreenPointToRay(Input.mousePosition);
            if (terrainCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                Destroy(moveToPoint);
                moveToPoint = (GameObject)Instantiate(moveTo, hit.point + halfHeight, new Quaternion());
                StopAllCoroutines();
                StartCoroutine(MoveTowardsWherePlayerClicked(hit.point + halfHeight));
            }
        }
    }

    private IEnumerator MoveTowardsWherePlayerClicked(Vector3 wherePlayerClickedToMove)
    {
        while(transform.position != wherePlayerClickedToMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, wherePlayerClickedToMove, Time.deltaTime * 25);

            yield return null;
        }
        Destroy(moveToPoint);
    }
}
