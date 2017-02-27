using UnityEngine;
using System.Collections;

public class LucianW : Skillshot
{
    [SerializeField]
    private GameObject lineAreaAfterHit;

    [SerializeField]
    private float lineAreaAfterHitDuration = 0.5f;

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
        projectileToShoot.GetComponentInChildren<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, false, lineAreaAfterHit, lineAreaAfterHitDuration, true);

        SkillDone();
    }
}
