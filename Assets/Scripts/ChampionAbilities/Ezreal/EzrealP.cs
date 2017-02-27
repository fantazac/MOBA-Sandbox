using UnityEngine;
using System.Collections;

public class EzrealP : Buff
{
    private int maxStacks = 5;

    private int stacks;

    private float attackSpeedPerStack = 0.1f;

    protected override void Start()
    {
        base.Start();

        duration = 6;

        //chaque fois qu'un skillshot frappe un ennemi, on gagne un stack et reset la durée
    }

    public override void ActivateBuff()
    {
        isActive = true;
        if(stacks < maxStacks)
        {
            stacks++;
            playerMovement.BasicAttack.SetAttackSpeed(attackSpeedPerStack);
        }

        StopAllCoroutines();
        StartCoroutine(DestroyBuffOnDurationFinished());
    }

    public override void ConsumeBuff()
    {
        playerMovement.BasicAttack.SetAttackSpeed(-attackSpeedPerStack * stacks);
        stacks = 0;
        StopAllCoroutines();
        isActive = false;
    }

}
