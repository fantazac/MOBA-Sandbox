using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public Health health;
    [HideInInspector]
    public MovementSpeed movementSpeed;
    [HideInInspector]
    public float range;

    private void Start()
    {
        health = GetComponent<Health>();
        movementSpeed = GetComponent<MovementSpeed>();
    }
}
