using UnityEngine;
using System.Collections;

public class OutOfCombat : MonoBehaviour
{
    private Player player;

    public bool isOutOfCombat;
    private float timeOutOfCombat;
    private float timeRequiredToGetOutOfCombat;

    public delegate void PlayerIsOutOfCombatHandler();
    public event PlayerIsOutOfCombatHandler PlayerIsOutOfCombat;

    public delegate void PlayerIsNowInCombatHandler();
    public event PlayerIsNowInCombatHandler PlayerIsNowInCombat;

    private void Start()
    {
        player = GetComponent<Player>();
        if (player.PhotonView.isMine)
        {
            player.PlayerStats.Health.OnDamageTaken += SetPlayerInCombat;
        }
        

        timeRequiredToGetOutOfCombat = 6;
        
        if (player.PhotonView.isMine)
        {
            SetPlayerInCombat();
        }
    }

    public bool IsOutOfCombat()
    {
        return isOutOfCombat;
    }

    public void SetPlayerInCombat()
    {
        if (isOutOfCombat)
        {
            player.PhotonView.RPC("SetPlayerInCombatOnServer", PhotonTargets.AllViaServer);
        }
        
        isOutOfCombat = false;
        StopAllCoroutines();
        StartCoroutine(PutPlayerOutOfCombat());
    }

    [PunRPC]
    protected void SetPlayerInCombatOnServer()
    {
        if (PlayerIsNowInCombat != null)
        {
            PlayerIsNowInCombat();
        }
        isOutOfCombat = false;
    }

    [PunRPC]
    protected void SetPlayerOutOfCombatOnServer()
    {
        if (PlayerIsOutOfCombat != null)
        {
            PlayerIsOutOfCombat();
        }
        isOutOfCombat = true;
    }

    protected IEnumerator PutPlayerOutOfCombat()
    {
        timeOutOfCombat = 0;

        while (timeRequiredToGetOutOfCombat > timeOutOfCombat)
        {
            yield return null;

            timeOutOfCombat += Time.deltaTime;
        }

        player.PhotonView.RPC("SetPlayerOutOfCombatOnServer", PhotonTargets.AllViaServer);
    }
}
