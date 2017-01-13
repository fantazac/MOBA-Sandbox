using UnityEngine;
using System.Collections;

public class EzrealW : Skillshot
{
    protected override void Start()
    {
        range = 22;
        speed = 35;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
