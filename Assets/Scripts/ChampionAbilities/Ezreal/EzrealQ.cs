using UnityEngine;
using System.Collections;

public class EzrealQ : Skillshot
{
    protected override void Start()
    {
        range = 25;
        speed = 50;
        activateSkillMethodName = "UseEzrealQFromServer";
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealQFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }
}
