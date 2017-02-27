using UnityEngine;
using System.Collections;

public class LucianBasicAttack : BasicAttack
{
    private LucianP passive;

    protected override void Start()
    {
        base.Start();
        passive = (LucianP)player.GetPassiveSkill();
    }

    protected override IEnumerator AllowMovementIfFollowingTarget()
    {
        yield return new WaitForSeconds(timeAfterAttackForMovement);

        ShootPassiveShot();

        StartCoroutine(CheckMovementEachFrame());
    }

    private void ShootPassiveShot()
    {
        if (passive.IsActive())
        {
            passive.ConsumeBuff();
            CreateProjectile();
        }
    }
}
