using UnityEngine;
using System.Collections;

public class Dash : Movement
{
    protected float dashSpeed;

    protected override void Start()
    {
        AbilityType = AbilityType.BLINK;
        base.Start();
    }

    protected override IEnumerator SkillEffect()
    {
        Vector3 target = FindPointToMoveTo(transform.position);
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * dashSpeed);

            if (playerMovement != null)
            {
                playerMovement.PlayerMovingWithSkill();
            }

            yield return null;
        }

        SkillDone();
    }
}
