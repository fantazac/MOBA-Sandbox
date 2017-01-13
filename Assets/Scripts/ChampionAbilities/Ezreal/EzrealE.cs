using UnityEngine;
using System.Collections;

public class EzrealE : Blink
{
    protected override void Start()
    {
        maxDistance = 8;
        minDistance = 0;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
