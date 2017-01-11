﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;

    [SerializeField]
    private GameObject playerParent;

    [SerializeField]
    private GameObject[] spawners;

    private PlayerMovement playerMovement;
    private GameObject playerTemplate;

    private bool inChampSelect = false;

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
        /*if(playerMovement != null)
        {
            GUILayout.Label(playerMovement.timeStoppedMoving.ToString());
        }*/

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
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        inChampSelect = true;
    }

    private void SpawnMyPlayer(string champName)
    {
        //int random = Random.Range(0, spawners.Length);
        //Vector3 spawner = spawners[random].transform.position;
        Vector3 spawner = spawners[(PhotonNetwork.playerList.Length - 1) % 2].transform.position;
        playerTemplate = (GameObject)Instantiate(playerParent, new Vector3(), new Quaternion());
        GameObject player = PhotonNetwork.Instantiate(champName, spawner - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.parent = playerTemplate.transform;
        player.transform.parent.GetChild(0).gameObject.SetActive(true);
        player.transform.parent.GetChild(1).gameObject.SetActive(true);
        playerMovement = player.GetComponent<PlayerMovement>();
        //player.GetComponent<Player>().team = (Team)random;
        playerMovement.Player.SetTeam((Team)((PhotonNetwork.playerList.Length - 1) % 2));
        //player.transform.parent.GetChild(2).gameObject.SetActive(true);

        foreach (PlayerBase pb in player.GetComponents<PlayerBase>())
        {
            pb.enabled = true;
        }

        menuCamera.SetActive(false);
    }
}
