using UnityEngine;
using System.Collections;

public class Ezreal : Player
{
    protected override void Start()
    {
        InitialiseStats();

        base.Start();
    }

    protected override void InitialiseStats()
    {
        PlayerStats.movementSpeed = 325;
        PlayerStats.range = 550;
        BasicAttack.SetAttackSpeed(0.1f);

        AdjustStats();
    }

    protected override void AdjustStats()
    {
        PlayerStats.movementSpeed /= 100f;
        PlayerStats.range /= 100f;
    }
}
