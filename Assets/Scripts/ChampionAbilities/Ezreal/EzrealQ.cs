using UnityEngine;
using System.Collections;

public class EzrealQ : PlayerSkill
{
    [SerializeField]
    private GameObject projectile;

    private float range = 25;
    private float speed = 50;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 0;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealQFromServer(Vector3 mousePositionOnCast)
    {
        InfoReceivedFromServer(mousePositionOnCast);
    }

    protected override void UseSkill(Vector3 mousePositionOnCast)
    {
        SkillBegin();
        StartCoroutine(SkillEffectWithCastTime(mousePositionOnCast));
    }

    public override void ActivateSkill()
    {
        usingSkillFromThisView = true;
        playerMovement.PhotonView.RPC("UseEzrealQFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
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
