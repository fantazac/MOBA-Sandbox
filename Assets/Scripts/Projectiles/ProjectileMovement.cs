using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    private float speed = 0;
    private float range = 0;

    private Vector3 initialPosition;

    private WaitForSeconds delayProjectileAfterHit;

    public void ShootProjectile(float speed, float range)
    {
        this.speed = speed;
        this.range = range;
        initialPosition = transform.position;
        StartCoroutine(Shoot());
    }

    public void ShootProjectile(float speed, float range, GameObject projectileAfterHit, float projectileAfterHitDuration)
    {
        this.speed = speed;
        this.range = range;
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
}
