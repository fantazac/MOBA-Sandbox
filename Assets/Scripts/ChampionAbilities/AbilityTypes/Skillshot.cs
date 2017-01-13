using UnityEngine;
using System.Collections;

public abstract class Skillshot : PlayerSkill
{
    [SerializeField]
    protected GameObject projectile;

    protected float range;
    protected float speed;

    protected override void Start()
    {
        AbilityType = AbilityType.SKILLSHOT;
        base.Start();
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.Player.CanCastSpell(this);
    }

    protected override void UseSkillFromServer()
    {
        SkillBegin();
        playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnCast);

        if (delayCastTime == null)
        {
            StartCoroutine(SkillEffect());
        }
        else
        {
            StartCoroutine(SkillEffectWithCastTime());
        }
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;

        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(playerMovement.PhotonView, playerMovement.Player.team, speed, range);

        SkillDone();
    }
}
