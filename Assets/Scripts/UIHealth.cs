using UnityEngine;
using System.Collections;

public class UIHealth : MonoBehaviour
{
    [SerializeField]
    private Health health;

    private float maxLength;
    private float currentLength;

    private float barHeight;

    private void Start()
    {
        maxLength = transform.localScale.x;
        barHeight = transform.localScale.y;
    }

    private void Update()
    {
        transform.localScale = (Vector3.up * barHeight) + (Vector3.right * health.GetHealthPercent());
    }

}
