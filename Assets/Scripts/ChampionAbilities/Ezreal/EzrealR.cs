using UnityEngine;
using System.Collections;

public class EzrealR : PlayerSkill
{

    [SerializeField]
    private GameObject projectile;

    private float range = 710;
    private float speed = 36;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 3;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealRFromServer(Vector3 mousePositionOnCast)
    {
        InfoReceivedFromServer(mousePositionOnCast);
    }

    public override void ActivateSkill()
    {
        usingSkillFromThisView = true;
        playerMovement.PhotonView.RPC("UseEzrealRFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
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
