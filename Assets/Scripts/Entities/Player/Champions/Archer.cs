using UnityEngine;
using System.Collections;

public class Archer : Player
{

    protected override void Start()
    {
        InitialiseStats();

        base.Start();
    }

    protected override void InitialiseStats()
    {
        PlayerStats.movementSpeed.SetMovementSpeedOnSpawn(330);
        PlayerStats.range = 550;
        BasicAttack.SetAttackSpeedOnSpawn(0.625f, 0.14f);

        AdjustStats();
    }

    protected override void AdjustStats()
    {
        PlayerStats.range /= 100f;
    }

}
