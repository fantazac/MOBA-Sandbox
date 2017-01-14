using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PlayerBase
{
    public float movementSpeed = 325;

    public List<PlayerSkill> skills;

    protected override void Start()
    {
        base.Start();
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
