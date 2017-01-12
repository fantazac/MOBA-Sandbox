using UnityEngine;
using System.Collections;

public class LucianR : Skillshot
{
    [SerializeField]
    private int amountOfBullets = 20;

    [SerializeField]
    private float durationOfActive = 2.5f;

    private int bulletsShot = 0;

    //to change, only works when facing up
    private Vector3 offsetBetweenProjectiles = Vector3.right * 0.4f;

    private WaitForSeconds delayBetweenBullets;

    protected override void Start()
    {
        delayBetweenBullets = new WaitForSeconds(durationOfActive / (float)amountOfBullets);

        range = 22;
        speed = 32;
        activateSkillMethodName = "UseLucianRFromServer";
        cancelSkillMethodName = "CancelLucianRFromServer";
        base.Start();
    }

    //receive skill info from server in Player with skills list from playerMovement (only 1 method)
    [PunRPC]
    protected void UseLucianRFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }
    // ^
    [PunRPC]
    protected void CancelLucianRFromServer()
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
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player.team, speed, range);

        bulletsShot++;
    }
}
