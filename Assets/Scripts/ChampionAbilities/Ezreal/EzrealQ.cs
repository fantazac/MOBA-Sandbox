using UnityEngine;
using System.Collections;

public class EzrealQ : Skillshot
{
    protected override void Start()
    {
        range = 25;
        speed = 50;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
