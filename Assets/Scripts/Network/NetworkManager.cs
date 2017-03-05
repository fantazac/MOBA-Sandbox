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
    private GameObject playerHealthBar;

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
            StaticObjects.PlayerCamera = null;
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
            if (GUILayout.Button("Archer"))
            {
                SpawnMyPlayer("Archer");
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
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        //LOAD ALL INFO NEEDED FROM ALL PLAYERS BEFORE SHOWING CHAMP SELECT
        //ex. all healths, all positions, all skillshots positions, all creeps, etc

        //doesnt work if someone disconnects and another person tries to connect 
        //ex. Player0 on, Player1 on, Player0 leaves, next player will be Player1 when there is one already, no Player0
        playerId = PhotonNetwork.playerList.Length - 1;

        if (playerId >= maxNumberOfPlayers)
        {
            Debug.Log("Game is full. Press 'ESCAPE' and try again.");
        }
        else
        {
            inChampSelect = true;
            StartCoroutine(LoadPlayers());
        }      
    }

    private IEnumerator LoadPlayers()
    {
        yield return null;
        
        foreach (GameObject playerToGetInfo in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerToGetInfo.GetComponent<Player>().SendInfo();
        }
    }

    private void SpawnMyPlayer(string champName)
    {
        Vector3 spawner = spawners[playerId % 2].transform.position;
        playerTemplate = (GameObject)Instantiate(playerParent, new Vector3(), new Quaternion());
        GameObject healthBar = PhotonNetwork.Instantiate("HealthBar", new Vector3(), playerHealthBar.transform.rotation, 0);
        healthBar.transform.SetParent(playerTemplate.transform);
        GameObject player = PhotonNetwork.Instantiate(champName, spawner - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.parent = playerTemplate.transform;
        player.transform.parent.GetChild(0).gameObject.SetActive(true);
        player.transform.parent.GetChild(1).gameObject.SetActive(true);
        player.transform.parent.GetChild(2).gameObject.SetActive(true);
        StaticObjects.Player = player.GetComponent<Player>();
        StaticObjects.Player.PlayerMovement.spawnPoint = spawner - (Vector3.up * 1.5f);
        StaticObjects.Player.PlayerMovement.EntityTeam.SetTeam((Team)(playerId % 2));
        StaticObjects.Player.SetPlayerId(playerId);
        StaticObjects.PlayerCamera = player.transform.parent.GetChild(0).gameObject.GetComponent<Camera>();
        healthBar.GetComponent<UIFollowPlayer>().SetPlayerToHealthBar(StaticObjects.Player, playerId);

        foreach (PlayerBase pb in player.GetComponents<PlayerBase>())
        {
            pb.enabled = true;
        }

        menuCamera.SetActive(false);
    }
}
