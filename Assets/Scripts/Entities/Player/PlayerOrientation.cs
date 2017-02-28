using UnityEngine;
using System.Collections;

public class PlayerOrientation : PlayerBase
{
    private Vector3 networkMove;

    [SerializeField]
    private int rotationSpeed = 15;

    private Vector3 rotationAmountLastFrame;
    private Vector3 rotationAmount;

    protected override void Start()
    {
        base.Start();
    }

    private bool CanRotate()
    {
        foreach (PlayerSkill ps in Player.skills)
        {
            if (ps != null && !ps.canRotateWhileCasting && ps.skillIsActive)
            {
                return false;
            }
        }
        return true;
    }

    public void RotatePlayer(Vector3 clickedPosition)
    {
        StopAllCoroutines();
        StartCoroutine(Rotate(clickedPosition));
    }

    public void RotatePlayerInstantly(Vector3 clickedPosition)
    {
        transform.rotation = Quaternion.LookRotation((clickedPosition - transform.position).normalized);
    }

    public void RotatePlayerUntilReachedTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(RotateUntilReachedTarget(target));
    }

    private IEnumerator Rotate(Vector3 clickedPosition)
    {
        rotationAmount = Vector3.up;
        rotationAmountLastFrame = Vector3.zero;
        
        while (rotationAmountLastFrame != rotationAmount)
        {
            if (CanRotate())
            {
                rotationAmountLastFrame = rotationAmount;

                rotationAmount = Vector3.RotateTowards(transform.forward, clickedPosition - transform.position, Time.deltaTime * rotationSpeed, 0);

                transform.rotation = Quaternion.LookRotation(rotationAmount);
            }

            yield return null;
        }
    }

    private IEnumerator RotateUntilReachedTarget(Transform target)
    {
        rotationAmount = Vector3.up;
        rotationAmountLastFrame = Vector3.zero;
        Health enemyHealth = target.GetComponent<Health>();
        if(enemyHealth == null)
        {
            Debug.Log("Target has no health? " + target.name);
        }

        while (target != null && !enemyHealth.IsDead())
        {
            if (CanRotate())
            {
                rotationAmountLastFrame = rotationAmount;

                rotationAmount = Vector3.RotateTowards(transform.forward, target.position - transform.position, Time.deltaTime * rotationSpeed, 0);

                transform.rotation = Quaternion.LookRotation(rotationAmount);
            }

            yield return null;
        }
    }
}
