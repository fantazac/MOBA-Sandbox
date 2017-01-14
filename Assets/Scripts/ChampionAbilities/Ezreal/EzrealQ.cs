using UnityEngine;
using System.Collections;

public class EzrealQ : Skillshot
{
    protected override void Start()
    {
        range = 1150;
        speed = 2000;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
