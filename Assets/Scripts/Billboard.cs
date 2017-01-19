using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = transform.parent.GetComponentInChildren<Camera>();
        transform.LookAt(playerCamera.transform);
    }
}
