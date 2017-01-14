using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    public int movementSpeed = 8;
    public float maxHealth = 100;
    public float currentHealth;
    public Team team;
    public float respawnTime = 5;

    public List<PlayerSkill> skills;

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

    [PunRPC]
    protected void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        skills[skillId].InfoReceivedFromServerToUseSkill(mousePositionOnCast);
    }

    [PunRPC]
    protected void CancelSkillFromServer(int skillId)
    {
        skills[skillId].InfoReceivedFromServerToCancelSkill();
    }

    public void ChangedHealthOnServer(float damage)
    {
        PhotonView.RPC("OnHealthChanged", PhotonTargets.All, damage);
    }

    public bool CanCastSpell(PlayerSkill skill)
    {
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null && ps.skillActive && !ps.castableSpellsWhileActive.Contains(skill))
            {
                return false;
            }
        }
        return true;
    }

    [PunRPC]
    private void OnHealthChanged(float damage)
    {
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
