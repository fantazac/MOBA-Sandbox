using UnityEngine;
using System.Collections;

public class LucianW : Skillshot
{
    [SerializeField]
    private float projectileAfterHitDuration = 0.5f;

    protected override void Start()
    {
        range = 900;
        speed = 1550;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;

        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.EntityTeam.Team, speed, range, projectileAfterHitDuration);

        SkillDone();
    }
}
