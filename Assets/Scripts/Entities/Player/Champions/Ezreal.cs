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
        movementSpeed = 325;
        range = 550;

        AdjustStats();
    }

    protected override void AdjustStats()
    {
        movementSpeed /= 100f;
        range /= 100f;
    }
}
