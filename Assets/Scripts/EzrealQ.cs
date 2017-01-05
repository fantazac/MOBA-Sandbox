using UnityEngine;
using System.Collections;

public class EzrealQ : PlayerSkill
{
    [SerializeField]
    private GameObject projectile;

    private float range = 25;
    private float speed = 50;

    RaycastHit hit;

    //private bool activated = false;

    protected override void Start()
    {
        skillId = 0;
        delayCastTime = new WaitForSeconds(castTime);
        base.Start();
    }

    public override void ActivateSkill()
    {
        //activated = true;
        SkillBeingUsed(skillId, hit.point + playerMovement.halfHeight);
        StartCoroutine(SkillCastTime());
    }

    public override bool CanUseSkill(Vector3 mousePosition)
    {
        return playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity);
    }

    protected override IEnumerator SkillCastTime()
    {
        yield return delayCastTime;

        GameObject projectileToShoot = PhotonNetwork.Instantiate("EzrealQ", transform.position, transform.rotation, 0);
        projectileToShoot.GetComponent<ProjectileMovement>().ShootProjectile(speed, range);

        SkillDone(skillId);
    }

    /*private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (activated)
        {
            if (stream.isWriting)
            {
                activated = false;
                stream.SendNext(420);
            }
        }
        else
        {
            if ((int)stream.ReceiveNext() == 420)
            {
                SkillBeingUsed(skillId, hit.point + playerMovement.halfHeight);
                StartCoroutine(SkillCastTime());
            }
        }

    }*/
}
