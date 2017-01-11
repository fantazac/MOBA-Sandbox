using UnityEngine;
using System.Collections;

public class LucianR : PlayerSkill
{

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private int amountOfBullets = 20;

    private int bulletsShot = 0;

    [SerializeField]
    private float durationOfSkill = 2.5f;

    private Vector3 offsetBetweenProjectiles = Vector3.right * 0.4f;

    private WaitForSeconds delayBetweenBullets;

    private float range = 22;
    private float speed = 32;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 3;
        delayBetweenBullets = new WaitForSeconds(durationOfSkill / (float)amountOfBullets);
        base.Start();
    }

    [PunRPC]
    protected void UseLucianRFromServer(Vector3 mousePositionOnCast)
    {
        SkillBegin();
        StartCoroutine(SkillEffect(mousePositionOnCast));
    }

    [PunRPC]
    protected void CancelLucianRFromServer()
    {
        StopAllCoroutines();
        bulletsShot = 0;
        SkillDone();
    }

    public override void ActivateSkill()
    {
        playerMovement.PhotonView.RPC("UseLucianRFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
    }

    public override void CancelSkill()
    {
        playerMovement.PhotonView.RPC("CancelLucianRFromServer", PhotonTargets.All);
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.CanCastSpell(this);
    }

    protected override IEnumerator SkillEffect(Vector3 mousePositionOnTerrain)
    {
        playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnTerrain);

        Shoot();

        while(bulletsShot < amountOfBullets)
        {
            yield return delayBetweenBullets;

            Shoot();
        }

        bulletsShot = 0;
        SkillDone();
    }

    private void Shoot()
    {
        //bugged, shoot correctly only if looking up
        GameObject projectileToShoot = (GameObject)Instantiate(projectile,
                transform.position + (bulletsShot % 2 == 0 ? offsetBetweenProjectiles : -offsetBetweenProjectiles), transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player.team, speed, range);

        bulletsShot++;
    }
}
