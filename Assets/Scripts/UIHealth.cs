using UnityEngine;
using System.Collections;

public class UIHealth : MonoBehaviour
{
    private Health health;

    private RectTransform rectTransform;

    private float maxLength;
    private float currentLength;

    private void Start()
    {
        health = transform.parent.parent.parent.GetComponentInChildren<Health>();
        rectTransform = GetComponent<RectTransform>();
        maxLength = rectTransform.rect.width;
    }

    private void Update()
    {
        rectTransform.sizeDelta.Set(maxLength * health.GetHealthPercent(), rectTransform.rect.height);
    }

}
