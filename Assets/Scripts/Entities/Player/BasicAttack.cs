using UnityEngine;
using System.Collections;

public class BasicAttack : MonoBehaviour
{
    private const float TWO_FRAMES = 0.066666666f;

    [SerializeField]
    private GameObject basicAttackProjectile;

    private float baseAttackSpeed;

    [HideInInspector]
    public float attackSpeed; //# attacks per second

    private float realAttackSpeed; //duration of a full attack cycle

    private float delayPercentBeforeAttack;

    private float timeBeforeAttack;
    private float timeAfterAttack;
    private float timeAfterAttackForMovement;

    private Player player;
    private Health targetHealth;

    private bool canBasicAttack;
    private bool queueAttack;

    private int selectedTargetId = -1;

    private void Start()
    {
        player = GetComponent<Player>();
        player.PlayerAttackMovement.IsInRangeForBasicAttack += UseBasicAttack;

        canBasicAttack = true;
    }

    public void SetAttackSpeedOnSpawn(float baseAttackSpeedToSet, float delayPercentBeforeAttack)
    {
        this.delayPercentBeforeAttack = delayPercentBeforeAttack;
        SetAttackSpeed(baseAttackSpeedToSet);
    }

    public void SetAttackSpeed(float baseAttackSpeed)
    {
        this.baseAttackSpeed = baseAttackSpeed;
        attackSpeed = baseAttackSpeed;
        realAttackSpeed = 1f / attackSpeed;
        timeBeforeAttack = realAttackSpeed * delayPercentBeforeAttack;
        timeAfterAttack = realAttackSpeed * (1 - delayPercentBeforeAttack);
        timeAfterAttackForMovement = timeBeforeAttack + TWO_FRAMES;
    }

    private void UseBasicAttack(int targetId)
    {
        player.PhotonView.RPC("UseBasicAttackOnServer", PhotonTargets.AllViaServer, targetId);
    }

    [PunRPC]
    private void UseBasicAttackOnServer(int targetId)
    {
        if (selectedTargetId != targetId)
        {
            targetHealth = FindEnemyPlayer(targetId).GetComponent<Health>();
        }

        if (targetHealth != null && !targetHealth.IsDead())
        {
            queueAttack = true;
            selectedTargetId = targetId;
            if (canBasicAttack)
            {
                StopAllCoroutines();
                StartCoroutine(Attack());
            }
            else
            {
                StartCoroutine(CheckMovementEachFrame());
            }
        }
    }

    public GameObject FindEnemyPlayer(int enemyPlayerId)
    {
        GameObject enemyPlayer = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().PlayerId == enemyPlayerId)
            {
                enemyPlayer = player;
                break;
            }
        }

        return enemyPlayer;
    }

    //need to call this when the target dies
    public void CancelBasicAttack()
    {
        if (canBasicAttack)
        {
            StopAllCoroutines();
        }
        queueAttack = false;
    }

    private IEnumerator CheckMovementEachFrame()
    {
        while (queueAttack)
        {
            yield return null;

            if (!player.PlayerAttackMovement.IsInRange(targetHealth.gameObject.transform) &&
                !player.PlayerAttackMovement.WasMovingBeforeSkill())
            {
                player.PlayerAttackMovement.UseAttackMove(selectedTargetId);
                CancelBasicAttack();
            }
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(timeBeforeAttack);

        canBasicAttack = false;

        GameObject basicAttackProjectileToShoot = (GameObject)Instantiate(basicAttackProjectile, transform.position + (transform.forward * 0.6f), transform.rotation);
        basicAttackProjectileToShoot.GetComponent<ProjectileBasicAttack>().ShootBasicAttack(player.PhotonView, targetHealth.gameObject, selectedTargetId, 2000);

        StartCoroutine(ResetAttack());
        StartCoroutine(AllowMovementIfFollowingTarget());
    }

    private IEnumerator AllowMovementIfFollowingTarget()
    {
        yield return new WaitForSeconds(timeAfterAttackForMovement);

        StartCoroutine(CheckMovementEachFrame());

    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeAfterAttack);

        canBasicAttack = true;

        if (queueAttack && !targetHealth.IsDead())
        {
            UseBasicAttackOnServer(selectedTargetId);
        }
    }
}
