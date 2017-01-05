using UnityEngine;
using System.Collections;

public class NetworkCharacterMovement : Photon.MonoBehaviour
{
    [SerializeField]
    private float positionTransitionSpeed;
    [SerializeField]
    private float rotationTransitionSpeed;

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
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * positionTransitionSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, Time.deltaTime * rotationTransitionSpeed);
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
