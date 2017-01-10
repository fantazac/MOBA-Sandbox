using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private Player player;

    private bool cameraLockedOnPlayer = true;
    private bool cameraFollowsPlayer = false;

    private Vector3 initialPosition;

    private void Start()
    {
        player = transform.parent.GetChild(2).gameObject.GetComponent<Player>();
        player.PlayerInput.OnPressedY += SetCameraLock;
        player.PlayerInput.OnPressedSpace += SetCameraOnPlayer;
        player.PlayerInput.OnReleasedSpace += SetCameraFree;
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
