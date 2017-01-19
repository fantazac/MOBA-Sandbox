using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileMovement : MonoBehaviour
{
    private float speed = 0;
    private float range = 0;
    private float damage = 10;
    public bool deleteOnHit = true;
    private PhotonView photonView;
    [HideInInspector]
    public Team sourceTeam;
    private bool projectileAfterHit = false;

    private List<GameObject> targetsAlreadyHit = new List<GameObject>();

    private Vector3 initialPosition;

    private WaitForSeconds delayProjectileAfterHit;

    public void ShootProjectile(PhotonView photonView, Team sourceTeam, float speed, float range)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    public void ShootProjectile(PhotonView photonView, Team sourceTeam, float speed, float range, float projectileAfterHitDuration)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        projectileAfterHit = true;
        delayProjectileAfterHit = new WaitForSeconds(projectileAfterHitDuration);
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    public void SetupProjectileAfterHit(PhotonView photonView, Team sourceTeam, List<GameObject> targetsAlreadyHit)
    {
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        this.targetsAlreadyHit = targetsAlreadyHit;
    }

    private IEnumerator Shoot()
    {
        while (Vector3.Distance(transform.position, initialPosition) < range)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            yield return null;
        }

        if (projectileAfterHit)
        {
            StartCoroutine(ShootProjectileAfterHit());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShootProjectileAfterHit()
    {
        if (delayProjectileAfterHit != null)
        {
            GameObject shotProjectileAfterHit = (GameObject)Instantiate(transform.GetChild(0).gameObject, transform.position, transform.rotation);
            shotProjectileAfterHit.GetComponentInChildren<ProjectileMovement>().SetupProjectileAfterHit(photonView, sourceTeam, targetsAlreadyHit);
            shotProjectileAfterHit.SetActive(true);
            
            transform.position = Vector3.down * 5;

            yield return delayProjectileAfterHit;

            Destroy(shotProjectileAfterHit);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Health targetHealth = collider.gameObject.GetComponent<Health>();
        
        if (targetHealth != null && targetHealth.GetComponent<EntityTeam>().Team != sourceTeam && CanHitTarget(collider.gameObject))
        {
            targetsAlreadyHit.Add(collider.gameObject);
            if (photonView.isMine)
            {
                //if the projectile gives a stat/heals (ex. EzrealW gives AS), changed this
                targetHealth.DamageTargetOnServer(damage);
            }
            
            if (deleteOnHit)
            {
                Destroy(gameObject);
            }
            else if(projectileAfterHit)
            {
                StopAllCoroutines();
                StartCoroutine(ShootProjectileAfterHit());
            }
        }
    }

    private bool CanHitTarget(GameObject target)
    {
        foreach(GameObject targetAlreadyHit in targetsAlreadyHit)
        {
            if(targetAlreadyHit == target)
            {
                return false;
            }
        }
        return true;
    }
}
