using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    [SerializeField]
    private int playerId;

    public int PlayerId { get { return playerId; } }

    public float movementSpeed = 325;

    public List<PlayerSkill> skills;

    protected override void Start()
    {
        base.Start();
    }

    public void SetPlayerId(int playerId)
    {
        PhotonView.RPC("SetPlayerOnNetwork", PhotonTargets.AllBufferedViaServer, playerId);
    }

    [PunRPC]
    private void SetPlayerOnNetwork(int playerId)
    {
        this.playerId = playerId;
    }

    [PunRPC]
    protected void UseSkillFromServer(int skillId, Vector3 mousePositionOnCast)
    {
        skills[skillId].InfoReceivedFromServerToUseSkill(mousePositionOnCast);
    }

    [PunRPC]
    protected void CancelSkillFromServer(int skillId)
    {
        skills[skillId].InfoReceivedFromServerToCancelSkill();
    }

    public bool CanCastSpell(PlayerSkill skill)
    {
        foreach (PlayerSkill ps in skills)
        {
            if (ps != null && ps.skillActive && !ps.castableSpellsWhileActive.Contains(skill))
            {
                return false;
            }
        }
        return true;
    }
}
