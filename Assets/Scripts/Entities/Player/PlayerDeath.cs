using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    private Player player;

    private Text respawnText;

    private float respawnTime = 8;
    private float currentRespawnTimer = 0;

    public delegate void RespawnPlayerHandler();
    public event RespawnPlayerHandler RespawnPlayer;

    private void Start()
    {
        player = StaticObjects.Player;

        if (player == GetComponent<Player>())
        {
            player.GetComponent<Health>().OnDeath += PlayerIsDead;
            respawnText = transform.parent.GetChild(1).GetChild(2).GetComponent<Text>();
            respawnText.gameObject.SetActive(false);
        }
    }

    private void PlayerIsDead()
    {
        if(currentRespawnTimer <= 0)
        {
            currentRespawnTimer = respawnTime;
            if (player == GetComponent<Player>())
            {
                StartCoroutine(Respawn());
            }
            else
            {
                StartCoroutine(RespawnWithoutTimer());
            }
        }
    }

    private IEnumerator Respawn()
    {
        respawnText.gameObject.SetActive(true);
        while (currentRespawnTimer > 0)
        {
            currentRespawnTimer -= Time.deltaTime;

            respawnText.text = ((int)currentRespawnTimer + 1).ToString();

            yield return null;
        }
        respawnText.text = "";
        respawnText.gameObject.SetActive(false);
        RespawnPlayer();
    }

    private IEnumerator RespawnWithoutTimer()
    {
        while (currentRespawnTimer > 0)
        {
            currentRespawnTimer -= Time.deltaTime;

            yield return null;
        }
        RespawnPlayer();
    }

}
