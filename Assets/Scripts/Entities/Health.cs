using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    private PhotonView photonView;

    public delegate void OnDeathHandler();
    public event OnDeathHandler OnDeath; 

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        SetToMaxHealthOnSpawn();
        if(GetComponent<Player>() != null)
        {
            GetComponent<PlayerDeath>().RespawnPlayer += SetToMaxHealthOnSpawn;
        }
    }

    public void DamageTargetOnServer(float damage)
    {
        photonView.RPC("DamageTargetFromServer", PhotonTargets.AllBufferedViaServer, damage);
    }

    public void HealTargetOnServer(float heal)
    {
        photonView.RPC("HealTargetFromServer", PhotonTargets.AllBufferedViaServer, heal);
    }

    [PunRPC]
    private void DamageTargetFromServer(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if(OnDeath != null)
            {
                OnDeath();
            }
            
        }
    }

    [PunRPC]
    private void HealTargetFromServer(float heal)
    {
        currentHealth += heal;
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public void SetToMaxHealthOnSpawn()
    {
        currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
