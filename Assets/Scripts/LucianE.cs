using UnityEngine;
using System.Collections;

public class LucianE : MonoBehaviour
{
    private float maxDistance = 6;
    private float minDistance = 2;

    private float dashSpeed = 32;

    private float cooldown = 2;
    private WaitForSeconds delayCooldown;

    private bool canUse = true;

    private PlayerMovement playerMovement;
    private InputManager inputManager;

    public delegate void LucianEActivatedHandler();
    public event LucianEActivatedHandler LucianEActivated;

    public delegate void LucianEFinishedHandler();
    public event LucianEFinishedHandler LucianEFinished;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.OnPressedE += ActivateLucianE;
        playerMovement = GetComponent<PlayerMovement>();
        delayCooldown = new WaitForSeconds(cooldown);
    }

    private void ActivateLucianE(Vector3 mousePosition)
    {
        if (canUse)
        {
            RaycastHit hit;
            if (playerMovement.terrainCollider.Raycast(playerMovement.GetRay(mousePosition), out hit, Mathf.Infinity))
            {
                canUse = false;
                LucianEActivated();
                StartCoroutine(MoveLucian(hit.point + playerMovement.halfHeight));
            }
        }
    }

    private Vector3 FindPointToDashTo(Vector3 mousePositionOnTerrain, Vector3 currentPosition)
    {
        float distanceBetweenBothVectors = Vector3.Distance(mousePositionOnTerrain, currentPosition);
        Vector3 normalizedVector = Vector3.Normalize(mousePositionOnTerrain - currentPosition);

        return distanceBetweenBothVectors > maxDistance ?
            (maxDistance * normalizedVector + currentPosition) :
            distanceBetweenBothVectors < minDistance ?
            (minDistance * normalizedVector + currentPosition) : mousePositionOnTerrain;
    }

    private IEnumerator MoveLucian(Vector3 mousePositionOnTerrain)
    {
        Vector3 target = FindPointToDashTo(mousePositionOnTerrain, transform.position);

        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * dashSpeed);

            playerMovement.PlayerDashing();

            yield return null;
        }
        LucianEFinished();
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return delayCooldown;

        canUse = true;
    }
}
