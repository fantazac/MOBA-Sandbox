using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    private float speed = 0;
    private float range = 0;
    private float damage = 10;
    private bool deleteOnHit = true;
    private PhotonView photonView;
    private Team sourceTeam;

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

    public void ShootProjectile(PhotonView photonView, Team sourceTeam, float speed, float range, GameObject projectileAfterHit, float projectileAfterHitDuration)
    {
        this.speed = speed;
        this.range = range;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        delayProjectileAfterHit = new WaitForSeconds(projectileAfterHitDuration);
        initialPosition = transform.position;
        StartCoroutine(Shoot(projectileAfterHit));
    }

    private IEnumerator Shoot()
    {
        while (Vector3.Distance(transform.position, initialPosition) < range)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator Shoot(GameObject projectileAfterHit)
    {
        while (Vector3.Distance(transform.position, initialPosition) < range)
        {
            transform.position += transform.forward * Time.deltaTime * speed;

            yield return null;
        }

        GameObject shotProjectileAfterHit = (GameObject)Instantiate(projectileAfterHit, transform.position, transform.rotation);

        transform.position = Vector3.down * 5;

        yield return delayProjectileAfterHit;

        Destroy(shotProjectileAfterHit);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(3);
        Player tempPlayer = collider.gameObject.GetComponent<Player>();
        if (tempPlayer != null && tempPlayer.team != sourceTeam)
        {
            if (photonView.isMine)
            {
                tempPlayer.ChangedHealthOnServer(damage);
            }
            
            if (deleteOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
