using UnityEngine;
using System.Collections;

public class EzrealR : Skillshot
{
    protected override void Start()
    {
        range = 250;
        speed = 36;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
