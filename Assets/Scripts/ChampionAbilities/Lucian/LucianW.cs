using UnityEngine;
using System.Collections;

public class LucianW : Skillshot
{
    [SerializeField]
    private GameObject projectileAfterHit;

    [SerializeField]
    private float projectileAfterHitDuration = 0.5f;

    protected override void Start()
    {
        range = 22;
        speed = 36;
        activateSkillMethodName = "UseLucianWFromServer";
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseLucianWFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;

        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player.team, speed, range, projectileAfterHit, projectileAfterHitDuration);

        SkillDone();
    }
}
