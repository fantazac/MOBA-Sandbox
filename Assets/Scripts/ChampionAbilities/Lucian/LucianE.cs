using UnityEngine;
using System.Collections;

public class LucianE : Dash
{
    protected override void Start()
    {
        maxDistance = 6;
        minDistance = 2;
        dashSpeed = 32;
        base.Start();
    }
}
