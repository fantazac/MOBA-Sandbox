using UnityEngine;
using System.Collections;

public class Blink : Movement
{
    protected override void Start()
    {
        AbilityType = AbilityType.BLINK;
        base.Start();
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        Vector3 target = FindPointToMoveTo(transform.position);

        yield return delayCastTime;

        //playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnCast);
        transform.position = target;
        playerMovement.NotifyPlayerMoved();

        SkillDone();
    }
}
