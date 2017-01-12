using UnityEngine;
using System.Collections;

public class EzrealW : Skillshot
{
    protected override void Start()
    {
        range = 22;
        speed = 35;
        activateSkillMethodName = "UseEzrealWFromServer";
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealWFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }
}
