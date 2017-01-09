using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    private float speed = 0;
    private float range = 0;

    private Vector3 initialPosition;

    public void ShootProjectile(float speed, float range)
    {
        this.speed = speed;
        this.range = range;
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
        Destroy(gameObject);
    }
}
