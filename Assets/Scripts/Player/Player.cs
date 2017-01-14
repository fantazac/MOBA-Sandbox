using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    public float movementSpeed = 325;
    public Team team;
    public float respawnTime = 5;

    public List<PlayerSkill> skills;

    private WaitForSeconds delayDeath;

    protected override void Start()
    {
        delayDeath = new WaitForSeconds(respawnTime);
        base.Start();
    }

    public void SetTeam(Team team)
    {
        PhotonView.RPC("SetTeamOnNetwork", PhotonTargets.AllBufferedViaServer, team);
    }

    [PunRPC]
    private void SetTeamOnNetwork(Team team)
    {
        this.team = team;
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
        //PhotonView.RPC("OnHealthChanged", PhotonTargets.AllBufferedViaServer, damage);
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
        /*currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            transform.position = PlayerMovement.halfHeight;
            PlayerMovement.PlayerMovingWithSkill();
            //StartCoroutine(RespawnAfterDeath());
        }*/
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
