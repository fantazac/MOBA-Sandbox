using UnityEngine;
using System.Collections;

public class LucianBasicAttack : BasicAttack
{
    private LucianP passive;
    private bool shootPassiveAfterAttack;

    protected override void Start()
    {
        base.Start();
        passive = (LucianP)player.GetPassiveSkill();
    }

    protected override void VerifyBeforeAttack()
    {
        shootPassiveAfterAttack = DetermineIfShootPassive();
    }

    protected override IEnumerator AllowMovementIfFollowingTarget()
    {
        yield return new WaitForSeconds(timeAfterAttackForMovement);

        if (shootPassiveAfterAttack)
        {
            shootPassiveAfterAttack = false;
            passive.ConsumeBuff();
            CreateProjectile();
        }

        StartCoroutine(CheckMovementEachFrame());
    }

    private bool DetermineIfShootPassive()
    {
        return passive.IsActive();
    }
}
