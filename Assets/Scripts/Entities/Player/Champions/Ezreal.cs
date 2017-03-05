using UnityEngine;
using System.Collections;

public class Ezreal : Player
{
    private EzrealP ezrealPassive;

    protected override void Start()
    {
        InitialiseStats();

        base.Start();

        ezrealPassive = (EzrealP)passive;
    }

    protected override void InitialiseStats()
    {
        PlayerStats.MovementSpeed.SetMovementSpeedOnSpawn(325);
        PlayerStats.range = 550;
        BasicAttack.SetAttackSpeedOnSpawn(0.625f, 0.14f);

        AdjustStats();
    }

    protected override void AdjustStats()
    {
        PlayerStats.range /= 100f;
    }

    public override void ProjectileHitEnemyTarget()
    {
        ezrealPassive.ActivateBuff();
        base.ProjectileHitEnemyTarget();
    }

    public override void ProjectileHitAllyTarget()
    {
        ezrealPassive.ActivateBuff();
    }
}
