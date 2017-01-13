using UnityEngine;
using System.Collections;

public class Movement : PlayerSkill
{
    protected float maxDistance;
    protected float minDistance;

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.Player.CanCastSpell(this);
    }

    protected override void UseSkillFromServer()
    {
        SkillBegin();

        if (delayCastTime == null)
        {
            StartCoroutine(SkillEffect());
        }
        else
        {
            StartCoroutine(SkillEffectWithCastTime());
        }
    }

    protected Vector3 FindPointToMoveTo(Vector3 currentPosition)
    {
        float distanceBetweenBothVectors = Vector3.Distance(mousePositionOnCast, currentPosition);
        Vector3 normalizedVector = Vector3.Normalize(mousePositionOnCast - currentPosition);

        return distanceBetweenBothVectors > maxDistance ?
            (maxDistance * normalizedVector + currentPosition) :
            distanceBetweenBothVectors < minDistance ?
            (minDistance * normalizedVector + currentPosition) : mousePositionOnCast;
    }
}
