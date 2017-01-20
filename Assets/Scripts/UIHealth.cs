using UnityEngine;
using System.Collections;

public class UIHealth : MonoBehaviour
{
    public Health health;

    private void Update()
    {
        if(health != null)
        {
            transform.localScale = Vector3.up + (Vector3.right * health.GetHealthPercent());
        }
    }
}
