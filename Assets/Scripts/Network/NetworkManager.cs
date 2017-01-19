using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;

    [SerializeField]
    private GameObject playerParent;

    [SerializeField]
    private GameObject[] spawners;

    [SerializeField]
    private int maxNumberOfPlayers = 10;

    private GameObject playerTemplate;

    private bool inChampSelect = false;

    private int playerId = 0;

    private void Start()
    {
        Connect();
    }

    private void Update()
    {
        //shows connected players
        //Debug.Log(PhotonNetwork.playerList.Length);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StaticObjects.Player = null;
            Destroy(playerTemplate);
            menuCamera.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("MOBA v1.0.0");
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label(PhotonNetwork.GetPing().ToString());
        GUILayout.Label(playerId.ToString());

        if (inChampSelect)
        {
            /*if (GUILayout.Button("Test"))
            {
                SpawnMyPlayer("_Test");
                inChampSelect = false;
            }*/
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
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        //doesnt work if someone disconnects and another person tries to connect
        playerId = PhotonNetwork.playerList.Length - 1;

        if (playerId >= maxNumberOfPlayers)
        {
            Debug.Log("Game is full. Press 'ESCAPE' and try again.");
        }
        else
        {
            inChampSelect = true;
        }      
    }

    private void SpawnMyPlayer(string champName)
    {
        Vector3 spawner = spawners[playerId % 2].transform.position;
        playerTemplate = (GameObject)Instantiate(playerParent, new Vector3(), new Quaternion());
        GameObject player = PhotonNetwork.Instantiate(champName, spawner - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.parent = playerTemplate.transform;
        player.transform.parent.GetChild(0).gameObject.SetActive(true);
        player.transform.parent.GetChild(1).gameObject.SetActive(true);
        player.transform.parent.GetChild(2).gameObject.SetActive(true);
        StaticObjects.Player = player.GetComponent<Player>();
        StaticObjects.Player.PlayerMovement.EntityTeam.SetTeam((Team)(playerId % 2));
        StaticObjects.Player.SetPlayerId(playerId);

        foreach (PlayerBase pb in player.GetComponents<PlayerBase>())
        {
            pb.enabled = true;
        }

        menuCamera.SetActive(false);
    }
}
