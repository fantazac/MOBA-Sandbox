using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;

    [SerializeField]
    private GameObject[] spawners;

    private void Start()
    {
        Connect();
    }

    private void Update()
    {
        Debug.Log(PhotonNetwork.playerList.Length);
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
        GameObject player = PhotonNetwork.Instantiate("Player", spawners[Random.Range(0, spawners.Length)].transform.position - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.GetChild(1).gameObject.SetActive(true);
        menuCamera.SetActive(false);
    }
}
