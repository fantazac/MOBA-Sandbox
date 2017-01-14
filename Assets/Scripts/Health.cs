using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentHealth = maxHealth;
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
            Debug.Log("Dead target");
            //Dead
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
}
