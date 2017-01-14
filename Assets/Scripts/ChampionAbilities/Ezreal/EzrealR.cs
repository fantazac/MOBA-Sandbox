using UnityEngine;
using System.Collections;

public class EzrealR : Skillshot
{
    protected override void Start()
    {
        range = (float)Range.GLOBAL;
        speed = 2000;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
