using UnityEngine;
using System.Collections;

public class Player : PlayerBase
{

    public float maxHealth = 100;
    public float currentHealth;
    public Team team;
    public float respawnTime = 5;

    private WaitForSeconds delayDeath;

    protected override void Start()
    {
        delayDeath = new WaitForSeconds(respawnTime);
        currentHealth = maxHealth;
        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 15;
        base.Start();
    }

    public void SetTeam(Team team)
    {
        this.team = team;
        PhotonView.RPC("SetTeamOnNetwork", PhotonTargets.OthersBuffered, team);
    }

    [PunRPC]
    private void SetTeamOnNetwork(Team team)
    {
        this.team = team;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //SerializeState(stream, info);
        PlayerMovement.SerializeState(stream, info);
    }

    public void ChangedHealthOnServer(float damage)
    {
        Debug.Log(1);
        PhotonView.RPC("OnHealthChanged", PhotonTargets.All, damage);
    }

    [PunRPC]
    private void OnHealthChanged(float damage)
    {
        Debug.Log(2);
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            transform.position = PlayerMovement.halfHeight;
            PlayerMovement.PlayerMovingWithSkill();
            //StartCoroutine(RespawnAfterDeath());
        }
    }

    /*private IEnumerator RespawnAfterDeath()
    {
        yield return delayDeath;
    }*/

    /*public override void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            float oldHealth = currentHealth;
            currentHealth = (float)stream.ReceiveNext();

            //if(oldHealth != currentHealth)
            //{
            //    OnHealthChanged();
            //}
        }
    }*/
}
