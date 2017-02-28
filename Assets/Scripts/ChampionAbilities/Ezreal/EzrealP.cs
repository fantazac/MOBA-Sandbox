using UnityEngine;
using System.Collections;

public class EzrealP : Buff
{
    private int maxStacks = 5;

    private int stacks;

    private float attackSpeedPercentPerStack = 0.1f;

    protected override void Start()
    {
        base.Start();

        duration = 6;
    }

    [PunRPC]
    protected override void ActivateBuffOnServer()
    {
        isActive = true;
        if(stacks < maxStacks)
        {
            stacks++;
            playerMovement.BasicAttack.SetAttackSpeed(attackSpeedPercentPerStack);
        }

        StopAllCoroutines();
        StartCoroutine(DestroyBuffOnDurationFinished());
    }

    [PunRPC]
    protected override void ConsumeBuffOnServer()
    {
        playerMovement.BasicAttack.SetAttackSpeed(-attackSpeedPercentPerStack * stacks);
        stacks = 0;
        StopAllCoroutines();
        isActive = false;
    }

}
