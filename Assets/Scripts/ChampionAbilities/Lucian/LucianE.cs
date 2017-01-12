using UnityEngine;
using System.Collections;

public class LucianE : PlayerSkill
{
    private float maxDistance = 6;
    private float minDistance = 2;

    private float dashSpeed = 32;

    RaycastHit hit;

    protected override void Start()
    {
        base.Start();
    }

    public override void ActivateSkill()
    {
        usingSkillFromThisView = true;
        playerMovement.PhotonView.RPC("UseLucianEFromServer", PhotonTargets.All, hit.point + playerMovement.halfHeight);
    }

    [PunRPC]
    protected void UseLucianEFromServer(Vector3 mousePositionOnCast)
    {
        this.mousePositionOnCast = mousePositionOnCast;
        InfoReceivedFromServerToUseSkill();
    }

    protected override void UseSkill()
    {
        SkillBegin();
        StartCoroutine(SkillEffect());
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity) && playerMovement.CanCastSpell(this);
    }

    private Vector3 FindPointToDashTo(Vector3 currentPosition)
    {
        float distanceBetweenBothVectors = Vector3.Distance(mousePositionOnCast, currentPosition);
        Vector3 normalizedVector = Vector3.Normalize(mousePositionOnCast - currentPosition);

        return distanceBetweenBothVectors > maxDistance ?
            (maxDistance * normalizedVector + currentPosition) :
            distanceBetweenBothVectors < minDistance ?
            (minDistance * normalizedVector + currentPosition) : mousePositionOnCast;
    }

    protected override IEnumerator SkillEffect()
    {
        Vector3 target = FindPointToDashTo(transform.position);
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * dashSpeed);

            if(playerMovement != null)
            {
                playerMovement.PlayerMovingWithSkill();
            }

            yield return null;
        }

        SkillDone();
    }
}
