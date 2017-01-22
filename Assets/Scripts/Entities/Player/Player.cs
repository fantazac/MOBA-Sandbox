using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    [SerializeField]
    private int playerId;

    public int PlayerId { get { return playerId; } }

    private Health health;

    public float movementSpeed = 325;

    public List<PlayerSkill> skills;

    private string nextAction = "";
    private int nextSkillId;
    private Vector3 nextMousePosition;

    private bool infoSent = false;

    protected override void Start()
    {
        health = GetComponent<Health>();

        PlayerInput.OnPressedD += DoDamageToPlayer;
        PlayerInput.OnPressedF += DoHealToPlayer;
        foreach (PlayerSkill ps in skills)
        {
            if(ps != null)
            {
                ps.SkillFinished += UseNextAction;
            }
        }

        base.Start();
    }

    private void DoDamageToPlayer()
    {
        health.DamageTargetOnServer(10);
    }

    private void DoHealToPlayer()
    {
        health.HealTargetOnServer(10);
    }

    public void SetPlayerId(int playerId)
    {
        PhotonView.RPC("SetPlayerOnNetwork", PhotonTargets.AllBufferedViaServer, playerId);
    }

    private void SetNextAction(string actionName, int skillId, Vector3 mousePosition)
    {
        nextAction = actionName;
        nextSkillId = skillId;
        nextMousePosition = mousePosition;
    }

    private void UseNextAction()
    {
        if (nextAction != "")
        {
            SendActionToServer(nextAction, nextSkillId, nextMousePosition);
            nextAction = "";
        }
    }

    [PunRPC]
    private void SetPlayerOnNetwork(int playerId)
    {
        this.playerId = playerId;
    }

    public void SendActionToServer(string actionName, int skillId, Vector3 mousePosition)
    {
        // use Actions

        if (CanUseSkill(skillId) && (!skills[skillId].HasCastTime() || !infoSent))
        {
            infoSent = true;
            //change to AllBufferedViaServer when prediction is good (ex. ezreal ult server position has to be calculated)
            PhotonView.RPC(actionName, PhotonTargets.AllViaServer, skillId, mousePosition);
        }
        else
        {
            SetNextAction(actionName, skillId, mousePosition);
        }
    }

    [PunRPC]
    protected void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        infoSent = false;
        skills[skillId].InfoReceivedFromServerToUseSkill(mousePositionOnCast);
    }

    [PunRPC]
    protected void CancelSkillFromServer(int skillId)
    {
        skills[skillId].InfoReceivedFromServerToCancelSkill();
    }

    private bool CanUseSkill(int skillId)
    {
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null && ps.skillIsActive && skills[skillId].HasCastTime())
            {
                return false;
            }
        }
        return true;
    }
}
