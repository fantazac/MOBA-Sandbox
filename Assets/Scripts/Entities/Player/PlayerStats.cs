using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public Health Health;
    [HideInInspector]
    public MovementSpeed MovementSpeed;
    [HideInInspector]
    public float range;
    [HideInInspector]
    public OutOfCombat OutOfCombat;

    private void Start()
    {
        Health = GetComponent<Health>();
        MovementSpeed = GetComponent<MovementSpeed>();
        OutOfCombat = GetComponent<OutOfCombat>();
    }
}
