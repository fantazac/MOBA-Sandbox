using UnityEngine;
using System.Collections;

public class BasicAttack : MonoBehaviour
{
    private const float BASE_ATTACK_SPEED = 0.625f;

    [SerializeField]
    private GameObject basicAttackProjectile;

    [HideInInspector]
    public float baseAttackSpeed;

    [HideInInspector]
    public float attackSpeed; //# attacks per second

    private float realAttackSpeed; //duration of a full attack cycle

    private float delayPercentBeforeFirstAttack;

    private float delayBeforeAttack;
    private float delayAfterAttack;

    private Player player;

    private bool canBasicAttack;
    private bool queueAttack;

    private GameObject selectedTarget;

    private void Start()
    {
        player = GetComponent<Player>();
        player.PlayerAttackMovement.IsInRangeForBasicAttack += UseBasicAttack;

        canBasicAttack = true;
    }

    public void SetAttackSpeed(float delayPercentBeforeFirstAttack)
    {
        this.delayPercentBeforeFirstAttack = delayPercentBeforeFirstAttack;

        baseAttackSpeed = BASE_ATTACK_SPEED;
        attackSpeed = baseAttackSpeed;
        realAttackSpeed = 1f / attackSpeed;
    }

    private void UseBasicAttack(GameObject target)
    {
        queueAttack = true;
        if (canBasicAttack)
        {
            selectedTarget = target;
            StopAllCoroutines();
            StartCoroutine(Attack());
        }
    }

    public void CancelBasicAttack()
    {
        if (canBasicAttack)
        {
            StopAllCoroutines();
        }
        queueAttack = false;
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(realAttackSpeed * delayPercentBeforeFirstAttack);

        canBasicAttack = false;

        GameObject basicAttackProjectileToShoot = (GameObject)Instantiate(basicAttackProjectile, transform.position + (transform.forward * 0.6f), transform.rotation);
        basicAttackProjectileToShoot.GetComponent<ProjectileBasicAttack>().ShootBasicAttack(player.PhotonView, selectedTarget, 2000);

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(realAttackSpeed * (1 - delayPercentBeforeFirstAttack));

        canBasicAttack = true;

        if (queueAttack)
        {
            UseBasicAttack(selectedTarget);
        }
    }
}
