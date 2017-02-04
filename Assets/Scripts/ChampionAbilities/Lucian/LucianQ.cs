using UnityEngine;
using System.Collections;

public class LucianQ : PlayerSkill
{
    [SerializeField]
    protected GameObject lineArea;

    [SerializeField]
    protected float lineAreaDuration;

    protected float rangeToCast;
    protected float rangeOfSkill;

    private GameObject target;

    protected override void Start()
    {
        AbilityType = AbilityType.NA;
        rangeToCast = 500;
        rangeOfSkill = 900;
        delayCastTime = new WaitForSeconds(castTime);
        ModifyValues();
        base.Start();
    }

    protected override void ModifyValues()
    {
        rangeToCast /= DIVISION_FACTOR;
        rangeOfSkill /= DIVISION_FACTOR;
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return MouseIsOnEnemyTarget();
    }

    public override void InfoReceivedFromServerToUseSkill(Vector3 mousePositionOnCast)
    {
        target = playerMovement.PlayerAttackMovement.FindEnemyPlayer((int)mousePositionOnCast.x);
        if(Vector3.Distance(target.transform.position, transform.position) <= rangeToCast)
        {
            IsInRange();
        }
        else
        {
            playerMovement.PlayerAttackMovement.SetMoveTowardsUnfriendlyTarget(target.transform, rangeToCast);
            playerMovement.PlayerAttackMovement.IsInRangeForSkill += IsInRange;
        }
    }

    private void IsInRange()
    {
        playerMovement.StopMovement();
        base.InfoReceivedFromServerToUseSkill(Vector3.zero);
    }

    protected override void UseSkillFromServer()
    {
        SkillBegin();
        playerMovement.PlayerOrientation.RotatePlayerInstantly(target.transform.position);

        if (delayCastTime == null)
        {
            StartCoroutine(SkillEffect());
        }
        else
        {
            StartCoroutine(SkillEffectWithCastTime());
        }
    }

    protected override IEnumerator SkillEffectWithCastTime()
    {
        yield return delayCastTime;

        GameObject lineAreaToInstantiate = (GameObject)Instantiate(lineArea, transform.position + (transform.forward * lineArea.transform.localScale.z * 0.5f), transform.rotation);
        lineAreaToInstantiate.GetComponent<LineArea>().ActivateAoE(playerMovement.PhotonView, playerMovement.EntityTeam.Team, lineAreaDuration);

        SkillDone();
    }

}
