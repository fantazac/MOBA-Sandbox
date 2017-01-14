using UnityEngine;
using System.Collections;

public class LucianE : Dash
{
    protected override void Start()
    {
        maxDistance = 425;
        minDistance = 100;
        dashSpeed = 32;
        base.Start();
    }
}
