using UnityEngine;
using System.Collections;

public class NetworkCharacterMovement : Photon.MonoBehaviour
{
    Vector3 realPosition;
    Quaternion realRotation;

    private void Start()
    {
        realPosition = Vector3.zero;
        realRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * 9);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * 15);
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
