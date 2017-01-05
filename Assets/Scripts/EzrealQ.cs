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

    public override void ActivateSkill()
    {
        SkillBeingUsed(skillId, hit.point + playerMovement.halfHeight);
        StartCoroutine(SkillCastTime());
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity);
    }

    protected override IEnumerator SkillCastTime()
    {
        yield return delayCastTime;
        
        GameObject projectileToShoot = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(speed, range);

        SkillDone(skillId);
    }
}
