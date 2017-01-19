using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private int distanceFromBorderToStartMoving = 60;

    [SerializeField]
    private int speed = 28;

    private int screenWidth;
    private int screenHeight;

    private Player player;

    private bool cameraLockedOnPlayer = true;
    private bool cameraFollowsPlayer = false;

    private Vector3 initialPosition;

    private Vector3 mousePositionOnFrame;

    private void Start()
    {
        player = transform.parent.GetChild(3).gameObject.GetComponent<Player>();
        player.PlayerInput.OnPressedY += SetCameraLock;
        player.PlayerInput.OnPressedSpace += SetCameraOnPlayer;
        player.PlayerInput.OnReleasedSpace += SetCameraFree;
        
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        initialPosition = transform.position;
        transform.position += player.transform.position;
        player.PlayerMovement.PlayerMoved += FollowPlayer;
    }

    private void Update()
    {
        if (!CameraShouldFollowPlayer())
        {
            mousePositionOnFrame = Input.mousePosition;

            if(mousePositionOnFrame.x > (screenWidth - distanceFromBorderToStartMoving))
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else if (mousePositionOnFrame.x < distanceFromBorderToStartMoving)
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }

            if (mousePositionOnFrame.y > (screenHeight - distanceFromBorderToStartMoving))
            {
                transform.position += Vector3.forward * speed * Time.deltaTime;
            }
            else if (mousePositionOnFrame.y < distanceFromBorderToStartMoving)
            {
                transform.position += Vector3.back * speed * Time.deltaTime;
            }
        }
    }

    private void SetCameraOnPlayer()
    {
        cameraFollowsPlayer = true;
        FollowPlayer();
    }

    private void SetCameraFree()
    {
        cameraFollowsPlayer = false;
    }

    private void SetCameraLock()
    {
        cameraLockedOnPlayer = !cameraLockedOnPlayer;
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (CameraShouldFollowPlayer())
        {
            transform.position = player.transform.position + initialPosition;
        }
    }

    private bool CameraShouldFollowPlayer()
    {
        return cameraLockedOnPlayer || cameraFollowsPlayer;
    }
}
