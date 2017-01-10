using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour
{
    private Player player;

    private bool cameraLockedOnPlayer = true;
    private bool cameraFollowsPlayer = false;

    private Vector3 initialPosition;

    private void Start()
    {
        player = transform.parent.GetChild(3).gameObject.GetComponent<Player>();
        if (player != null)
        {

            player.PlayerInput.OnPressedY += SetCameraLock;
            player.PlayerInput.OnPressedSpace += SetCameraOnPlayer;
            player.PlayerInput.OnReleasedSpace += SetCameraFree;
            initialPosition = transform.position;
            transform.position += player.transform.position;
            player.GetComponent<PlayerMovement>().PlayerMoved += FollowPlayer;
        }
        else
        {
            throw new Exception("Player set to null reference");
        }
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
