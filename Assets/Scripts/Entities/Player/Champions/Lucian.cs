using UnityEngine;
using System.Collections;

public class Lucian : Player
{
    private bool askedServerForCulling;
    private bool askedServerForQ;

    private float distanceBetweenLucianAndMoveTargetBeforeUsingE;
    private Vector3 lastMoveBeforeUsingE;

    protected override void Start()
    {
        InitialiseStats();

        base.Start();
    }

    protected override void InitialiseStats()
    {
        PlayerStats.movementSpeed = 325;
        PlayerStats.range = 500;
        BasicAttack.SetAttackSpeed(0.1f);

        AdjustStats();
    }

    protected override void AdjustStats()
    {
        PlayerStats.movementSpeed /= 100f;
        PlayerStats.range /= 100f;
    }

    public override void SendActionToServer(Actions action, int skillId, Vector3 mousePosition)
    {
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SendActionToServer(Lucian) for this object: " + gameObject.name);
        }

        if (CanUseSkill(skillId) && (!skills[skillId].HasCastTime() || !infoSent))
        {
            if (skillId == 1 && askedServerForCulling || skillId == 2 && askedServerForQ)
            {
                return;
            }

            SetNextActionAfterCastingSkillWithCastTime(skillId);

            if (skillId == 3)
            {
                askedServerForCulling = true;
            }

            if (skillId == 0)
            {
                askedServerForQ = true;

                PhotonView.RPC("CancelMovementOnServer", PhotonTargets.AllViaServer);
                SendSkillInfoToServer(skillId, Vector3.zero, 
                    PlayerMouseSelection.selectedTargetForUseInNextFrame.GetComponent<Player>().PlayerId);
            }
            else
            {
                SendSkillInfoToServer(skillId, mousePosition, -1);
            }
        }
        else
        {
            PhotonView.RPC("CancelMovementOnServer", PhotonTargets.AllViaServer);
            SetNextAction(action, skillId, mousePosition);
        }
    }

    private bool DistanceToTravelAfterEIsBiggerThanBeforeTheCast()
    {
        return PlayerNormalMovement.WasMovingBeforeSkill() && PlayerNormalMovement.GetLastMove() == lastMoveBeforeUsingE &&
                    distanceBetweenLucianAndMoveTargetBeforeUsingE >= Vector3.Distance(transform.position, lastMoveBeforeUsingE);
    }

    public override void SetBackMovementAfterSkillWithoutCastTime()
    {
        if (!PhotonView.isMine)
        {
            Debug.Log("ERROR - This shouldn't be happening.. Check SetBackMovementAfterSkillWithoutCastTime(Lucian) for this object: " + gameObject.name);
        }

        if (nextAction == Actions.NONE)
        {
            if (skills[2].skillIsActive)
            {
                if (DistanceToTravelAfterEIsBiggerThanBeforeTheCast())
                {
                    PlayerNormalMovement.ActivateMovementTowardsPoint();
                }
                else if (PlayerAttackMovement.WasMovingBeforeSkill())
                {
                    PlayerAttackMovement.ActivateMovementTowardsUnfriendlyTarget();
                }
                else
                {
                    PlayerMovement.StopMovement();
                }

                distanceBetweenLucianAndMoveTargetBeforeUsingE = 0;
                lastMoveBeforeUsingE = Vector3.zero;
            }
            else
            {
                if (PlayerNormalMovement.WasMovingBeforeSkill())
                {
                    PlayerNormalMovement.ActivateMovementTowardsPoint();
                }
                else if (PlayerAttackMovement.WasMovingBeforeSkill())
                {
                    PlayerAttackMovement.ActivateMovementTowardsUnfriendlyTarget();
                }
            }
        }
    }

    [PunRPC]
    protected override void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        if (skillId == 3)
        {
            askedServerForCulling = false;
        }
        if (skillId == 0)
        {
            askedServerForQ = false;
        }
        if (skillId == 2 && PlayerNormalMovement.WasMovingBeforeSkill())
        {
            lastMoveBeforeUsingE = PlayerNormalMovement.GetLastMove();
            distanceBetweenLucianAndMoveTargetBeforeUsingE = Vector3.Distance(transform.position, lastMoveBeforeUsingE);
        }
        base.UseSkillFromServer(skillId, mousePositionOnCast);
    }
}
