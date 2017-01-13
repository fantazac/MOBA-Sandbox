using UnityEngine;
using System.Collections;

public class EzrealR : Skillshot
{
    protected override void Start()
    {
        range = 150;
        speed = 36;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }
}
