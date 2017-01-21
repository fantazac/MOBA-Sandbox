using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private UIFollowPlayer uiFollowPlayer;

    private void Start()
    {
        uiFollowPlayer.photonView.RPC("SetOrientation", PhotonTargets.AllBufferedViaServer);
    }

    [PunRPC]
    private void SetOrientation()
    {
        if (uiFollowPlayer.photonView.isMine)
        {
            transform.LookAt(StaticObjects.PlayerCamera.transform);
        }
        else
        {
            //transform.rotation = StaticObjects.Billboard.transform.rotation;
        }
    }
}