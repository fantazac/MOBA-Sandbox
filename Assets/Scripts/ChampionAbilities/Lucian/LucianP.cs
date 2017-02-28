using UnityEngine;
using System.Collections;

public class LucianP : Buff
{
    protected override void Start()
    {
        base.Start();

        duration = 3;

        foreach (PlayerSkill ps in playerMovement.Player.skills)
        {
            if (ps != null && ps != this)
            {
                ps.SkillFinished += ActivateBuff;
            }
        }
    }
}
