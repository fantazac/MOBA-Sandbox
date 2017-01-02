using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject[] blueSpawns;

    [SerializeField]
    private GameObject[] redSpawns;

    private int state = 0;

    private void Connect()
    {
        PhotonNetwork.ConnectToBestCloudServer("V1.0");
    }

    private void OnJoinedLobby()
    {
        state = 1;
    }

    private void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom()
    {
        state = 2;
    }

    private void Spawn(int team, string character)
    {
        state = 3;
        Debug.Log("You are on team... " + team + ". You are playing as... " + character);
    }

    private void OnGUI()
    {
        switch (state)
        {
            case 0:
                if (GUI.Button(new Rect(10, 10, 100, 30), "Connect"))
                {
                    Connect();
                }
                break;
            case 1:
                GUI.Label(new Rect(10, 10, 100, 30), "Connected");
                if (GUI.Button(new Rect(10, 10, 100, 30), "Search"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                break;
            case 2:
                GUI.Label(new Rect(10, 10, 100, 30), "Select Your Champion");
                if (GUI.Button(new Rect(70, 10, 100, 30), "Alex"))
                {
                    Spawn(0, "Alex");
                }
                break;
            case 3:
                break;
        }
    }
}
