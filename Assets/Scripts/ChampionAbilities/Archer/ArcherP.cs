using UnityEngine;
using System.Collections;

public class ArcherP : Buff
{
    private float movementSpeedPercentIncreaseValue;
    private int ticksBeforeMaxMovementSpeed;
    private float movementSpeedPercentIncreasePerTick;
    private float movementSpeedIncreaseDuration;

    private WaitForSeconds delayBetweenTicks;
    private int tickCount;

    protected override void Start()
    {
        base.Start();

        if (playerMovement.PhotonView.isMine)
        {
            playerMovement.PlayerStats.OutOfCombat.PlayerIsOutOfCombat += ActivateBuff;
            playerMovement.PlayerStats.OutOfCombat.PlayerIsNowInCombat += ConsumeBuff;
        }      

        ticksBeforeMaxMovementSpeed = 4;
        movementSpeedIncreaseDuration = 1;
        movementSpeedPercentIncreaseValue = 0.15f;
        delayBetweenTicks = new WaitForSeconds(movementSpeedIncreaseDuration / (float)ticksBeforeMaxMovementSpeed);
        movementSpeedPercentIncreasePerTick = movementSpeedPercentIncreaseValue / (float)ticksBeforeMaxMovementSpeed;
        hasNoDuration = true;
    }

    [PunRPC]
    protected override void ActivateBuffOnServer()
    {
        isActive = true;

        StopAllCoroutines();
        StartCoroutine(IncreaseMovementSpeedOverOneSecond());
    }

    private IEnumerator IncreaseMovementSpeedOverOneSecond()
    {
        tickCount = 0;

        while(tickCount < ticksBeforeMaxMovementSpeed)
        {
            yield return delayBetweenTicks;
            
            playerMovement.PlayerStats.MovementSpeed.SetMovementSpeed(movementSpeedPercentIncreasePerTick);
            tickCount++;
        }
    }

    [PunRPC]
    protected override void ConsumeBuffOnServer()
    {
        StopAllCoroutines();
        playerMovement.PlayerStats.MovementSpeed.SetMovementSpeed(-movementSpeedPercentIncreasePerTick * tickCount);
        //activate second part of passive if auto is unused
        isActive = false;
    }
}
