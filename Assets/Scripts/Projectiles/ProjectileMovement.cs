using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileMovement : MonoBehaviour
{
    private float speed = 0;
    private float range = 0;
    private float damage = 10;
    public bool deleteOnHit = true;
    private bool continueOnKill = false;
    private PhotonView photonView;
    [HideInInspector]
    public Team sourceTeam;
    private Player sourcePlayer;
    private bool canHitAllies;
    private bool lineAreaAfterHit = false;
    private GameObject lineAreaToActivateAfterHit;

    private List<GameObject> targetsAlreadyHit = new List<GameObject>();

    private Vector3 initialPosition;

    private float lineAreaAfterHitDuration;
    private bool lineAreaHasParent;

    public void ShootProjectile(PhotonView photonView, Player sourcePlayer, float speed, float range, bool canHitAllies)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourcePlayer = sourcePlayer;
        this.canHitAllies = canHitAllies;
        sourceTeam = sourcePlayer.EntityTeam.Team;
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    public void ShootProjectile(PhotonView photonView, Player sourcePlayer, float speed, float range, bool canHitAllies, bool continueOnKill)
    {
        this.continueOnKill = continueOnKill;
        ShootProjectile(photonView, sourcePlayer, speed, range, canHitAllies);
    }

    public void ShootProjectile(PhotonView photonView, Player sourcePlayer, float speed, float range, bool canHitAllies, GameObject lineAreaToActivateAfterHit, float lineAreaAfterHitDuration, bool lineAreaHasParent)
    {
        lineAreaAfterHit = true;
        this.lineAreaToActivateAfterHit = lineAreaToActivateAfterHit;
        this.lineAreaAfterHitDuration = lineAreaAfterHitDuration;
        this.lineAreaHasParent = lineAreaHasParent;
        ShootProjectile(photonView, sourcePlayer, speed, range, canHitAllies);
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

        if (targetHealth != null && CanHitTarget(collider.gameObject))
        {
            EntityTeam targetTeam = targetHealth.GetComponent<EntityTeam>();
            targetsAlreadyHit.Add(collider.gameObject);
            if(targetTeam.IsEnemy(sourceTeam) || (!targetTeam.IsEnemy(sourceTeam) && canHitAllies))
            {
                if (photonView.isMine)
                {
                    //if the projectile gives a stat/heals (ex. EzrealW gives AS), changed this
                    if (targetTeam.IsEnemy(sourceTeam))
                    {
                        sourcePlayer.ProjectileHitEnemyTarget();
                        targetHealth.DamageTargetOnServer(damage);
                    }
                    else if (canHitAllies)
                    {
                        sourcePlayer.ProjectileHitAllyTarget();
                        //buff, heal, etc
                    }
                }

                if (deleteOnHit)
                {
                    if (!continueOnKill || targetHealth.currentHealth > damage)
                    {
                        Destroy(gameObject);
                    }
                }
                else if (lineAreaAfterHit)
                {
                    ActivateAreaAfterHit();
                    Destroy(gameObject);
                }
            }
        }
    }

    private bool CanHitTarget(GameObject target)
    {
        foreach (GameObject targetAlreadyHit in targetsAlreadyHit)
        {
            if (targetAlreadyHit == target)
            {
                return false;
            }
        }
        return true;
    }
}
