using UnityEngine;
using System.Collections;

public class UIHealth : MonoBehaviour
{
    private Health health;

    private float maxLength;
    private float currentLength;

    private float barHeight;

    private void Start()
    {
        health = transform.parent.parent.GetComponentInChildren<Health>();
        maxLength = transform.localScale.x;
        barHeight = transform.localScale.y;
    }

    private void Update()
    {
        transform.localScale = (Vector3.up * barHeight) + (Vector3.right * health.GetHealthPercent());
    }

}
