using UnityEngine;
using System.Collections;

public class LineArea : MonoBehaviour
{
    private float damage = 10;
    private PhotonView photonView;
    [HideInInspector]
    public Team sourceTeam;

    private bool hitEnabled;

    private WaitForSeconds delayLineArea;

    public void ActivateAoE(PhotonView photonView, Team sourceTeam, float lineAreaDuration)
    {
        hitEnabled = true;
        this.photonView = photonView;
        this.sourceTeam = sourceTeam;
        delayLineArea = new WaitForSeconds(lineAreaDuration - (Time.deltaTime * 2));
        StartCoroutine(RemoveLineArea());
    }

    private IEnumerator RemoveLineArea()
    {
        yield return null; //2 frames
        yield return null;

        hitEnabled = false;

        yield return delayLineArea;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (hitEnabled)
        {
            Health targetHealth = collider.gameObject.GetComponent<Health>();

            if (targetHealth != null && targetHealth.GetComponent<EntityTeam>().Team != sourceTeam)
            {
                if (photonView.isMine)
                {
                    //if the projectile gives a stat/heals (ex. EzrealW gives AS), changed this
                    targetHealth.DamageTargetOnServer(damage);
                }
            }
        }
    }
}
