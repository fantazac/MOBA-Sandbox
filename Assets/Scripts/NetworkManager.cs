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
        Vector3 spawner = spawners[Random.Range(0, spawners.Length)].transform.position;
        GameObject playerTemplate = (GameObject)Instantiate(playerParent, new Vector3(), new Quaternion());
        GameObject player = PhotonNetwork.Instantiate("Player", spawner - (Vector3.up * 1.5f), new Quaternion(), 0);
        player.transform.parent = playerTemplate.transform;
        player.transform.parent.GetChild(0).gameObject.SetActive(true);
        player.transform.parent.GetChild(1).gameObject.SetActive(true);
        player.transform.parent.gameObject.GetComponentInChildren<SkillCooldown>().player = player;

        player.GetComponent<PlayerMovement>().enabled = true;
        foreach(PlayerSkill ps in player.GetComponents<PlayerSkill>())
        {
            ps.enabled = true;
        }
        player.GetComponent<InputManager>().enabled = true;
        
        menuCamera.SetActive(false);
    }
}
