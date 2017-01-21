using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastTimeBarManager : MonoBehaviour
{
    [SerializeField]
    private GameObject castTimeBar;

    [SerializeField]
    private Text abilityText;
     
    [SerializeField]
    private Text castTimeText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCastTimeBar(string abilityName, float castTime)
    {
        abilityText.text = abilityName;
        StartCoroutine(FillCastTimeBar(castTime));
    }

    private IEnumerator FillCastTimeBar(float castTime)
    {
        float castTimeLeft = castTime;
        while (true)
        {
            castTimeLeft -= Time.deltaTime;
            castTimeText.text = castTimeLeft.ToString("f1");
            castTimeBar.transform.localScale += Vector3.right * Time.deltaTime / castTime;
            if (castTimeBar.transform.localScale.x >= 1)
            {
                break;
            }
            yield return null;
        }
        abilityText.text = "";
        castTimeText.text = "";
        castTimeBar.transform.localScale = Vector3.up * castTimeBar.transform.localScale.y;
        gameObject.SetActive(false);
    }

}
