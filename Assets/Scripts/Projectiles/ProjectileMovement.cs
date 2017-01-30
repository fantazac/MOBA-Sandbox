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
    private bool lineAreaAfterHit = false;
    private GameObject lineAreaToActivateAfterHit;

    private List<GameObject> targetsAlreadyHit = new List<GameObject>();

    private Vector3 initialPosition;

    private float lineAreaAfterHitDuration;
    private bool lineAreaHasParent;

    public void ShootProjectile(PhotonView photonView, Team sourceTeam, float speed, float range)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    public void ShootProjectile(PhotonView photonView, Team sourceTeam, float speed, float range, GameObject lineAreaToActivateAfterHit, float lineAreaAfterHitDuration, bool lineAreaHasParent)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        lineAreaAfterHit = true;
        this.lineAreaToActivateAfterHit = lineAreaToActivateAfterHit;
        this.lineAreaAfterHitDuration = lineAreaAfterHitDuration;
        this.lineAreaHasParent = lineAreaHasParent;
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (Vector3.Distance(transform.position, initialPosition) < range)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            yield return null;
        }

        if (lineAreaAfterHit)
        {
            ActivateAreaAfterHit();
        }

        Destroy(gameObject);
    }

    private void ActivateAreaAfterHit()
    {
        GameObject lineArea = (GameObject)Instantiate(lineAreaToActivateAfterHit, transform.position, transform.rotation);

        foreach (LineArea area in lineArea.GetComponentsInChildren<LineArea>())
        {
            area.ActivateAoE(photonView, sourceTeam, lineAreaAfterHitDuration, targetsAlreadyHit, lineAreaHasParent);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Health targetHealth = collider.gameObject.GetComponent<Health>();
        
        if (targetHealth != null && targetHealth.GetComponent<EntityTeam>().IsEnemy(sourceTeam) && CanHitTarget(collider.gameObject))
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
            else if(lineAreaAfterHit)
            {
                ActivateAreaAfterHit();
                Destroy(gameObject);
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
