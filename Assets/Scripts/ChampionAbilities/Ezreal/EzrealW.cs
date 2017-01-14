using UnityEngine;
using System.Collections;

public class EzrealW : Skillshot
{
    protected override void Start()
    {
        range = 1000;
        speed = 1550;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
