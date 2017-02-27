using UnityEngine;
using System.Collections;

public class LucianR : Skillshot
{
    private int amountOfBullets = 20;

    private float durationOfActive = 3f;

    private int bulletsShot = 0;

    private float offset = 0.2f;

    private WaitForSeconds delayBetweenBullets;

    protected override void Start()
    {
        delayBetweenBullets = new WaitForSeconds(durationOfActive / (float)amountOfBullets);

        range = 1200;
        speed = 2000;
        base.Start();
    }

    protected override void CancelSkillFromServer()
    {
        StopAllCoroutines();
        bulletsShot = 0;
        SkillDone();
    }

    protected override void SkillDone()
    {
        if (playerMovement.PhotonView.isMine)
        {
            playerMovement.Player.SetBackMovementAfterSkillWithoutCastTime();
        }

        base.SkillDone();
    }

    protected override IEnumerator SkillEffect()
    {
        ShootOneBullet();

        while (bulletsShot < amountOfBullets)
        {
            yield return delayBetweenBullets;

            ShootOneBullet();
        }

        bulletsShot = 0;
        SkillDone();
    }

    private void ShootOneBullet()
    {
        GameObject projectileToShoot = (GameObject)Instantiate(projectile, 
            transform.position + (transform.forward * 0.6f) + (transform.right * (bulletsShot % 2 == 0 ? offset : -offset)), 
            transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player, speed, range, false);

        bulletsShot++;
    }
}
