using UnityEngine;
using System.Collections;

public class Dash : Movement
{
    protected float dashSpeed;

    protected override void Start()
    {
        AbilityType = AbilityType.DASH;
        base.Start();
    }

    protected override void SkillDone()
    {
        if (playerMovement.PhotonView.isMine)
        {
            playerMovement.Player.SetBackMovementAfterSkillWithoutCastTime();
        }
        
        base.SkillDone();
    }

    protected override IEnumerator SkillEffect()
    {
        Vector3 target = FindPointToMoveTo(transform.position);
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * dashSpeed);

            if (playerMovement != null)
            {
                playerMovement.NotifyPlayerMoved();
            }

            yield return null;
        }
        SkillDone();
    }
}
