using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("MOBA v1.0.0");
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    private void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        SpawnMyPlayer();
    }

    private void SpawnMyPlayer()
    {
        PhotonNetwork.Instantiate("Player", Vector3.up, new Quaternion(), 0);
        menuCamera.SetActive(false);
    }
}
