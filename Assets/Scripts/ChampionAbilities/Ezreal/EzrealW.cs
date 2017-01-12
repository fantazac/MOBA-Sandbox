using UnityEngine;
using System.Collections;

public class EzrealW : PlayerSkill
{

    [SerializeField]
    private GameObject projectile;

    private float range = 22;
    private float speed = 35;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 1;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealWFromServer(Vector3 mousePositionOnCast)
    {
        InfoReceivedFromServer(mousePositionOnCast);
    }

    public override void ActivateSkill()
    {
        usingSkillFromThisView = true;
        playerMovement.PhotonView.RPC("UseEzrealWFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
    }

    protected override void UseSkill(Vector3 mousePositionOnCast)
    {
        SkillBegin();
        StartCoroutine(SkillEffectWithCastTime(mousePositionOnCast));
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
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player.team, speed, range);

        SkillDone();
    }

}
