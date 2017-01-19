using UnityEngine;
using System.Collections;

public class UIFollowPlayer : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = transform.parent.GetChild(3);
        player.gameObject.GetComponent<PlayerMovement>().PlayerMoved += MoveCameraOnPlayer;
        MoveCameraOnPlayer();
    }

    private void MoveCameraOnPlayer()
    {
        transform.position = player.position + (Vector3.up * 2.5f);
    }
}
