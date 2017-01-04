using UnityEngine;
using System.Collections;

public class CameraPlayerMovement : MonoBehaviour
{
    private GameObject player;

    private bool cameraLockedOnPlayer = true;
    private bool followPlayer = true;

    private Vector3 initialPosition;

    private void Start()
    {
        player = transform.parent.GetChild(0).gameObject;
        initialPosition = transform.position - player.transform.position;
        player.GetComponent<PlayerMovement>().PlayerMoved += FollowPlayer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cameraLockedOnPlayer = !cameraLockedOnPlayer;
        }

        followPlayer = cameraLockedOnPlayer || Input.GetKey(KeyCode.Space);
    }

    private void FollowPlayer()
    {
        if (followPlayer)
        {
            transform.position = player.transform.position + initialPosition;
        }
    }
}
