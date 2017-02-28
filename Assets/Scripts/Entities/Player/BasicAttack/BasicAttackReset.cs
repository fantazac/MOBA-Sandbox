using UnityEngine;
using System.Collections;

public class BasicAttackReset : MonoBehaviour
{
    public delegate void BasicAttackAvailableHandler();
    public event BasicAttackAvailableHandler BasicAttackAvailable;

    public void ResetBasicAttack(float timeAfterAttack)
    {
        StartCoroutine(ResetAttack(timeAfterAttack));
    }

    protected IEnumerator ResetAttack(float timeAfterAttack)
    {
        yield return new WaitForSeconds(timeAfterAttack);

        if (BasicAttackAvailable != null)
        {
            BasicAttackAvailable();
        }
    }
}
