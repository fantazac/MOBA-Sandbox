using UnityEngine;
using System.Collections;

public class MovementSpeed : MonoBehaviour
{

    private float baseMovementSpeed;
    public float movementSpeed;
    private float realMovementSpeed;

    public void SetMovementSpeedOnSpawn(float baseMovementSpeed)
    {
        this.baseMovementSpeed = baseMovementSpeed;
        movementSpeed = baseMovementSpeed;
        SetMovementSpeed(0);
    }

    public void SetMovementSpeed(float movementSpeedPercentChange)
    {
        movementSpeed += (baseMovementSpeed * movementSpeedPercentChange);
        realMovementSpeed = movementSpeed * 0.01f;
    }

    public void SetMovementSpeedForNewPlayer(float currentMovementSpeed)
    {
        movementSpeed = currentMovementSpeed;
        realMovementSpeed = movementSpeed * 0.01f;
    }

    public float GetRealMovementSpeed()
    {
        return realMovementSpeed;
    }

}
