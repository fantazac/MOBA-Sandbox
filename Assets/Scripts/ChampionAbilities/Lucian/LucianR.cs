using UnityEngine;
using System.Collections;

public class LucianR : Skillshot
{
    private int amountOfBullets = 20;

    private float durationOfActive = 3f;

    private int bulletsShot = 0;

    //to change, only works when facing up, right now shoots a line
    private Vector3 offsetBetweenProjectiles = Vector3.zero;//Vector3.right * 0.4f;

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
                transform.position + (bulletsShot % 2 == 0 ? offsetBetweenProjectiles : -offsetBetweenProjectiles), transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.EntityTeam.Team, speed, range);

        bulletsShot++;
    }
}
