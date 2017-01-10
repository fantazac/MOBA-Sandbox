using UnityEngine;
using System.Collections;

public class EzrealE : PlayerSkill
{
    private float maxDistance = 8;
    private float minDistance = 0;

    RaycastHit hit;

    protected override void Start()
    {
        skillId = 2;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    [PunRPC]
    protected void UseEzrealEFromServer(Vector3 mousePositionOnCast)
    {
        SkillBegin();
        StartCoroutine(SkillEffectWithCastTime(mousePositionOnCast));
    }

    public override void ActivateSkill()
    {
        playerMovement.PhotonView.RPC("UseEzrealEFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.CanCastSpell(this);
    }

    private Vector3 FindPointToTeleportTo(Vector3 mousePositionOnTerrain, Vector3 currentPosition)
    {
        float distanceBetweenBothVectors = Vector3.Distance(mousePositionOnTerrain, currentPosition);
        Vector3 normalizedVector = Vector3.Normalize(mousePositionOnTerrain - currentPosition);

        return distanceBetweenBothVectors > maxDistance ?
            (maxDistance * normalizedVector + currentPosition) :
            distanceBetweenBothVectors < minDistance ?
            (minDistance * normalizedVector + currentPosition) : mousePositionOnTerrain;
    }

    protected override IEnumerator SkillEffectWithCastTime(Vector3 mousePositionOnTerrain)
    {
        playerMovement.PlayerOrientation.RotatePlayerInstantly(mousePositionOnTerrain);
        Vector3 target = FindPointToTeleportTo(mousePositionOnTerrain, transform.position);

        yield return delayCastTime;

        transform.position = target;
        playerMovement.PlayerMovingWithSkill();

        SkillDone();
    }

}
