using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineArea : MonoBehaviour
{
    private float damage = 10;
    private PhotonView photonView;
    [HideInInspector]
    public Team sourceTeam;

    private bool hitEnabled;

    private WaitForSeconds delayLineArea;

    private List<GameObject> targetsAlreadyHit;

    private bool lineAreaHasParent;

    public void ActivateAoE(PhotonView photonView, Team sourceTeam, float lineAreaDuration)
    {
        hitEnabled = true;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        delayLineArea = new WaitForSeconds(lineAreaDuration - (Time.deltaTime * 2));
        StartCoroutine(RemoveLineArea());
    }

    public void ActivateAoE(PhotonView photonView, Team sourceTeam, float lineAreaDuration, List<GameObject> targetsAlreadyHit, bool lineAreaHasParent)
    {
        hitEnabled = true;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        this.targetsAlreadyHit = targetsAlreadyHit;
        this.lineAreaHasParent = lineAreaHasParent;
        delayLineArea = new WaitForSeconds(lineAreaDuration - (Time.deltaTime * 2));
        StartCoroutine(RemoveLineArea());
    }

    private IEnumerator RemoveLineArea()
    {
        yield return null; //2 frames
        yield return null;

        hitEnabled = false;

        yield return delayLineArea;

        Destroy(lineAreaHasParent ? transform.parent.gameObject : gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (hitEnabled)
        {
            Health targetHealth = collider.gameObject.GetComponent<Health>();

            if (targetHealth != null && targetHealth.GetComponent<EntityTeam>().Team != sourceTeam && CanHitTarget(collider.gameObject))
            {
                if (photonView.isMine)
                {
                    //if the projectile gives a stat/heals (ex. EzrealW gives AS), changed this
                    targetHealth.DamageTargetOnServer(damage);
                }
            }
        }
    }

    private bool CanHitTarget(GameObject target)
    {
        if(targetsAlreadyHit == null)
        {
            return true;
        }
        foreach (GameObject targetAlreadyHit in targetsAlreadyHit)
        {
            if (targetAlreadyHit == target)
            {
                return false;
            }
        }
        return true;
    }
}
