using UnityEngine;
using System.Collections;

public abstract class BasicAttack : MonoBehaviour
{
    protected const float TWO_FRAMES = 0.066666666f;

    [SerializeField]
    protected GameObject basicAttackProjectile;

    protected float baseAttackSpeed;

    [HideInInspector]
    public float attackSpeed; //# attacks per second

    protected float realAttackSpeed; //duration of a full attack cycle

    protected float delayPercentBeforeAttack;

    protected float timeBeforeAttack;
    protected float timeAfterAttack;
    protected float timeAfterAttackForMovement;

    protected Player player;
    protected Health targetHealth;

    protected bool canBasicAttack;
    protected bool queueAttack;

    protected int selectedTargetId = -1;

    public delegate void BasicAttackDoneHandler();
    public event BasicAttackDoneHandler BasicAttackDone;

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        player.PlayerAttackMovement.IsInRangeForBasicAttack += UseBasicAttack;

        canBasicAttack = true;
    }

    public void SetAttackSpeedOnSpawn(float baseAttackSpeedToSet, float delayPercentBeforeAttack)
    {
        this.delayPercentBeforeAttack = delayPercentBeforeAttack;
        baseAttackSpeed = baseAttackSpeedToSet;
        attackSpeed = baseAttackSpeed;
        SetAttackSpeed(0);
    }

    public void SetAttackSpeed(float attackSpeedChange)
    {
        attackSpeed += attackSpeedChange;
        realAttackSpeed = 1f / attackSpeed;
        timeBeforeAttack = realAttackSpeed * delayPercentBeforeAttack;
        timeAfterAttack = realAttackSpeed * (1 - delayPercentBeforeAttack);
        timeAfterAttackForMovement = timeBeforeAttack + TWO_FRAMES;
    }

    protected void UseBasicAttack(int targetId)
    {
        player.PhotonView.RPC("UseBasicAttackOnServer", PhotonTargets.AllViaServer, targetId);
    }

    [PunRPC]
    protected void UseBasicAttackOnServer(int targetId)
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

    protected IEnumerator CheckMovementEachFrame()
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

    protected IEnumerator Attack()
    {
        yield return new WaitForSeconds(timeBeforeAttack);

        canBasicAttack = false;

        CreateProjectile();

        if (BasicAttackDone != null)
        {
            BasicAttackDone();
        }

        StartCoroutine(ResetAttack());
        StartCoroutine(AllowMovementIfFollowingTarget());
    }

    protected void CreateProjectile()
    {
        GameObject basicAttackProjectileToShoot = (GameObject)Instantiate(basicAttackProjectile, transform.position + (transform.forward * 0.6f), transform.rotation);
        basicAttackProjectileToShoot.GetComponent<ProjectileBasicAttack>().ShootBasicAttack(player.PhotonView, targetHealth.gameObject, selectedTargetId, 2000);
    }

    protected virtual IEnumerator AllowMovementIfFollowingTarget()
    {
        yield return new WaitForSeconds(timeAfterAttackForMovement);

        StartCoroutine(CheckMovementEachFrame());

    }

    protected IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeAfterAttack);

        canBasicAttack = true;

        if (queueAttack && !targetHealth.IsDead())
        {
            UseBasicAttackOnServer(selectedTargetId);
        }
    }
}
