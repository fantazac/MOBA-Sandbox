using UnityEngine;
using System.Collections;

public class Buff : PlayerSkill
{

    protected float duration;
    protected bool isActive;
    protected bool hasNoDuration;

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

    public void ActivateBuff()
    {
        playerMovement.PhotonView.RPC("ActivateBuffOnServer", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    protected virtual void ActivateBuffOnServer()
    {
        isActive = true;
        StopAllCoroutines();
        if (!hasNoDuration)
        {
            StartCoroutine(DestroyBuffOnDurationFinished());
        }
    }

    protected IEnumerator DestroyBuffOnDurationFinished()
    {
        yield return new WaitForSeconds(duration);

        ConsumeBuff();
    }

    public void ConsumeBuff()
    {
        if (isActive)
        {
            playerMovement.PhotonView.RPC("ConsumeBuffOnServer", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    protected virtual void ConsumeBuffOnServer()
    {
        StopAllCoroutines();
        isActive = false;
    }

}
