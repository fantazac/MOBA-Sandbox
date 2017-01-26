using UnityEngine;
using System.Collections;

public class UIFollowPlayer : Photon.MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private void Start()
    {
        StaticObjects.Player.healthBar = gameObject;
    }

    public void SetPlayerToHealthBar(Player player, int playerId)
    {
        this.player = player.transform;

        photonView.RPC("SetPlayerToHealthBarOnServer", PhotonTargets.AllBufferedViaServer, playerId);
    }

    [PunRPC]
    private void SetPlayerToHealthBarOnServer(int playerId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].GetComponent<Player>().PlayerId == playerId)
            {
                player = players[i].transform;
                player.gameObject.GetComponent<PlayerMovement>().PlayerMoved += MoveCameraOnPlayer;
                GetComponentInChildren<UIHealth>().health = player.GetComponent<Health>();
                MoveCameraOnPlayer();
                break;
            }
        }
    }

    private void MoveCameraOnPlayer()
    {
        transform.position = player.position + (Vector3.up * 2.5f);
    }
}
