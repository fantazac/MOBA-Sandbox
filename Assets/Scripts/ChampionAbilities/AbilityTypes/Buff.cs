using UnityEngine;
using System.Collections;

public class Buff : PlayerSkill
{

    protected float duration;
    protected bool isActive;

    protected override void Start()
    {
        AbilityType = AbilityType.BUFF;
        base.Start();
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return false;
    }

    protected override void UseSkillFromServer()
    {
        //Keep empty
    }

    public bool IsActive()
    {
        return isActive;
    }

    protected void ActivateBuff()
    {
        isActive = true;
        StopAllCoroutines();
        StartCoroutine(DestroyBuffOnDurationFinished());
    }

    protected IEnumerator DestroyBuffOnDurationFinished()
    {
        yield return new WaitForSeconds(duration);
        
        ConsumeBuff();
    }

    public void ConsumeBuff()
    {
        StopAllCoroutines();
        isActive = false;
    }

}
