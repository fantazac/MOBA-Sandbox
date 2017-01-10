using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;

    [SerializeField]
    private GameObject playerParent;

    [SerializeField]
    private GameObject[] spawners;

    private bool inChampSelect = false;

    private void Start()
    {
        Connect();
    }

    //shows connected players
    /*private void Update()
    {
        Debug.Log(PhotonNetwork.playerList.Length);
    }*/

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("MOBA v1.0.0");
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label(PhotonNetwork.GetPing().ToString());
        if (inChampSelect)
        {
            if (GUILayout.Button("Test"))
            {
                SpawnMyPlayer("Player");
                inChampSelect = false;
            }
            if (GUILayout.Button("Ezreal"))
            {
                SpawnMyPlayer("Ezreal");
                inChampSelect = false;
            }
            if (GUILayout.Button("Lucian"))
            {
                SpawnMyPlayer("Lucian");
                inChampSelect = false;
            }
        }
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

        inChampSelect = true;
    }

    private void SpawnMyPlayer(string champName)
    {
        Vector3 spawner = spawners[Random.Range(0, spawners.Length)].transform.position;
        GameObject playerTemplate = (GameObject)Instantiate(playerParent, new Vector3(), new Quaternion());
        GameObject player = PhotonNetwork.Instantiate(champName, spawner - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.parent = playerTemplate.transform;
        player.transform.parent.GetChild(0).gameObject.SetActive(true);
        player.transform.parent.GetChild(1).gameObject.SetActive(true);
        //player.transform.parent.GetChild(2).gameObject.SetActive(true);

        foreach (PlayerBase pb in player.GetComponents<PlayerBase>())
        {
            pb.enabled = true;
        }
        
        menuCamera.SetActive(false);
    }
}
