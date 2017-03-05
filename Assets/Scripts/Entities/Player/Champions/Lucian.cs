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
        PlayerStats.movementSpeed.SetMovementSpeedOnSpawn(325);
        PlayerStats.range = 500;
        BasicAttack.SetAttackSpeedOnSpawn(0.638f, 0.14f);

        AdjustStats();
    }

    protected override void AdjustStats()
    {
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
            if (skillId == (int)SkillId.W && askedServerForCulling || skillId == (int)SkillId.E && askedServerForQ)
            {
                return;
            }

            SetNextActionAfterCastingSkillWithCastTime(skillId);

            if (skillId == (int)SkillId.R)
            {
                askedServerForCulling = true;
            }

            if (skillId == (int)SkillId.Q)
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
            if (skills[(int)SkillId.E].skillIsActive)
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
        if (skillId == (int)SkillId.R)
        {
            askedServerForCulling = false;
        }
        if (skillId == (int)SkillId.Q)
        {
            askedServerForQ = false;
        }
        if (skillId == (int)SkillId.E && PlayerNormalMovement.WasMovingBeforeSkill())
        {
            lastMoveBeforeUsingE = PlayerNormalMovement.GetLastMove();
            distanceBetweenLucianAndMoveTargetBeforeUsingE = Vector3.Distance(transform.position, lastMoveBeforeUsingE);
        }
        base.UseSkillFromServer(skillId, mousePositionOnCast);
    }
}
