using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileBasicAttack : MonoBehaviour
{

    private float speed = 0;
    private float damage = 10;
    private Player sourcePlayer;
    private PhotonView photonView;
    private int targetId;

    public void ShootBasicAttack(PhotonView photonView, Player sourcePlayer, GameObject target, int targetId, float speed)
    {
        this.sourcePlayer = sourcePlayer;
        this.speed = speed / 100f;
        this.photonView = photonView;
        this.targetId = targetId;
        transform.rotation = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
        StartCoroutine(Shoot(target));
    }

    private IEnumerator Shoot(GameObject target)
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.transform.position - transform.position, Time.deltaTime * speed, 0));

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Health targetHealth = collider.gameObject.GetComponent<Health>();
        Player targetPlayer = collider.gameObject.GetComponent<Player>();
        
        if (targetHealth != null && targetPlayer != null && targetPlayer.PlayerId == targetId)
        {
            if (photonView.isMine)
            {
                //if the projectile gives a stat/heals (ex. EzrealW gives AS), changed this
                
                sourcePlayer.ProjectileHitEnemyTarget();
                targetHealth.DamageTargetOnServer(damage);
            }

            Destroy(gameObject);
        }
    }
}
