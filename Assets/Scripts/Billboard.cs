using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public Camera playerCamera;

    public void SetCamera(Camera camera)
    {
        playerCamera = camera;
        transform.LookAt(playerCamera.transform);
    }

    public void SetCamera(Billboard localBillboard)
    {
        playerCamera = localBillboard.playerCamera;
        transform.rotation = localBillboard.transform.rotation;
    }
}