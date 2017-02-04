using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public Health health;
    [HideInInspector]
    public float movementSpeed;
    [HideInInspector]
    public float range;

    private void Start()
    {
        health = GetComponent<Health>();
    }
}
