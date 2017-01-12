using UnityEngine;
using System.Collections;

public class EzrealR : Skillshot
{
    protected override void Start()
    {
        range = 150;
        speed = 36;
        activateSkillMethodName = "UseEzrealRFromServer";
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealRFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }
}
