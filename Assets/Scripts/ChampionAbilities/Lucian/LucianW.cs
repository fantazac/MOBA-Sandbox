using UnityEngine;
using System.Collections;

public class LucianW : PlayerSkill
{

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private GameObject projectileAfterHit;

    [SerializeField]
    private float projectileAfterHitDuration = 0.5f;

    private float range = 22;
    private float speed = 36;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 1;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseLucianWFromServer(Vector3 mousePositionOnCast)
    {
        skillActive = true;
        StartCoroutine(SkillEffectWithCastTime(mousePositionOnCast));
    }

    public override void ActivateSkill()
    {
        playerMovement.PhotonView.RPC("UseLucianWFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.CanCastSpell(this);
    }

    protected override IEnumerator SkillEffectWithCastTime(Vector3 mousePositionOnTerrain)
    {
        playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnTerrain);

        yield return delayCastTime;

        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(speed, range, projectileAfterHit, projectileAfterHitDuration);

        SkillDone();
    }
}
