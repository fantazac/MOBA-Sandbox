using UnityEngine;
using System.Collections;

public class ArcherQ : Skillshot
{
    private float normalAngle;

    protected override void Start()
    {
        range = 900;
        speed = 2100;
        normalAngle = 20;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;
        
        GameObject projectileToShoot1 = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot1.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, false, true);

        GameObject projectileToShoot2 = (GameObject)Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, normalAngle, 0));
        projectileToShoot2.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, false, true);

        GameObject projectileToShoot3 = (GameObject)Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, -normalAngle, 0));
        projectileToShoot3.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, false, true);

        SkillDone();
    }
}
