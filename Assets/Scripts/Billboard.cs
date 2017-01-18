using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    private Transform player;

    private Vector3 positionRelativeToPlayer;

    private void Start()
    {
        player = transform.parent.parent.GetChild(2);
        positionRelativeToPlayer = transform.position - player.position;
    }

    private void Update()
    {
        transform.position = player.position;// - positionRelativeToPlayer;
        //transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);
    }


}
