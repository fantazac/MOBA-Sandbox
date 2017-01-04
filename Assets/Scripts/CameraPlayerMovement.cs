using UnityEngine;
using System.Collections;

public class CameraPlayerMovement : MonoBehaviour
{
    private GameObject player;
    private InputManager inputManager;

    private bool cameraLockedOnPlayer = true;
    private bool cameraFollowsPlayer = false;

    private Vector3 initialPosition;

    private void Start()
    {
        player = transform.parent.GetChild(2).gameObject;
        inputManager = player.GetComponent<InputManager>();
        inputManager.OnPressedY += SetCameraLock;
        inputManager.OnPressedSpace += SetCameraOnPlayer;
        inputManager.OnReleasedSpace += SetCameraFree;
        initialPosition = transform.position;
        transform.position += player.transform.position;
        player.GetComponent<PlayerMovement>().PlayerMoved += FollowPlayer;
    }

    private void SetCameraOnPlayer()
    {
        cameraFollowsPlayer = true;
    }

    private void SetCameraFree()
    {
        cameraFollowsPlayer = false;
    }

    private void SetCameraLock()
    {
        cameraLockedOnPlayer = !cameraLockedOnPlayer;
    }

    private void FollowPlayer()
    {
        if (cameraLockedOnPlayer || cameraFollowsPlayer)
        {
            transform.position = player.transform.position + initialPosition;
        }
    }
}
