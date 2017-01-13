using UnityEngine;
using System.Collections;

public class EzrealE : PlayerSkill
{
    private float maxDistance = 8;
    private float minDistance = 0;

    protected override void Start()
    {
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    protected override void UseSkillFromServer()
    {
        SkillBegin();
        StartCoroutine(SkillEffectWithCastTime());
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.Player.CanCastSpell(this);
    }

    private Vector3 FindPointToTeleportTo(Vector3 currentPosition)
    {
        float distanceBetweenBothVectors = Vector3.Distance(mousePositionOnCast, currentPosition);
        Vector3 normalizedVector = Vector3.Normalize(mousePositionOnCast - currentPosition);

        return distanceBetweenBothVectors > maxDistance ?
            (maxDistance * normalizedVector + currentPosition) :
            distanceBetweenBothVectors < minDistance ?
            (minDistance * normalizedVector + currentPosition) : mousePositionOnCast;
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnCast);
        Vector3 target = FindPointToTeleportTo(transform.position);

        yield return delayCastTime;

        transform.position = target;
        playerMovement.PlayerMovingWithSkill();

        SkillDone();
    }

}
