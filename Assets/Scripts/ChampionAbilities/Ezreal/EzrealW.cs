using UnityEngine;
using System.Collections;

public class EzrealW : Skillshot
{
    protected override void Start()
    {
        range = 1000;
        speed = 1550;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;

        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, true);

        SkillDone();
    }
}
