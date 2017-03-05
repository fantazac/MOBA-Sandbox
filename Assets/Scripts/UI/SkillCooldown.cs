using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillCooldown : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private List<UISkill> skills;

    private List<PlayerSkill> playerSkills;

    private void Start()
    {
        player = StaticObjects.Player;
        playerSkills = player.skills;
        player.PlayerInput.OnPressedSkill += SkillInputReceived;

        for (int i = 0; i < playerSkills.Count; i++)
        {
            if (playerSkills[i] != null) //remove when all champs have skills from start
            {
                playerSkills[i].uiSkill = skills[i];
                skills[i].cooldown = playerSkills[i].cooldown;
                skills[i].skillIcon.sprite = playerSkills[i].skillImage; //remove when we get image dynamically from folder
                skills[i].skillIcon.transform.parent.gameObject.GetComponent<Image>().sprite = playerSkills[i].skillImage;
                if (playerSkills[i].cooldownStartsOnCast)
                {
                    playerSkills[i].SetCooldownOnSkillStarted += SetCooldown;
                }
                else
                {
                    playerSkills[i].SetCooldownOnSkillFinished += SetCooldown;
                }
            }
        }
    }

    private void SetCooldown(int skillId)
    {
        skills[skillId].isOffCooldown = false;
        skills[skillId].cooldownLeft = skills[skillId].cooldown;
        skills[skillId].skillIcon.fillAmount = 0;
        StartCoroutine(SetSkillOffCooldown(skills[skillId], skillId));
    }

    private void SkillInputReceived(int skillId, Vector3 mousePosition)
    {
        if (CanUseSkill(skillId, mousePosition))
        {
            playerSkills[skillId].ActivateSkill();
        }
        else if (playerSkills[skillId].canBeCancelled && playerSkills[skillId].skillIsActive)
        {
            playerSkills[skillId].CancelSkill();
        }
    }

    private bool CanUseSkill(int skillId, Vector3 mousePosition)
    {
        return !StaticObjects.Player.PlayerStats.Health.IsDead() && 
            skills[skillId].CanUseSkill() && !playerSkills[skillId].skillIsActive && playerSkills[skillId].CanUseSkill(mousePosition);
    }

    private IEnumerator SetSkillOffCooldown(UISkill s, int skillId)
    {
        s.skillIcon.color = s.skillUnavailableColor;
        while (s.cooldownLeft > 0)
        {
            s.cooldownLeft -= Time.deltaTime;
            s.skillIcon.fillAmount = 1 - (s.cooldownLeft / s.cooldown);
            if(s.cooldownLeft >= 1)
            {
                s.cooldownText.text = ((int)s.cooldownLeft).ToString();
            }
            else if(s.cooldownLeft <= 0)
            {
                s.cooldownText.text = "";
            }
            else
            {
                s.cooldownText.text = s.cooldownLeft.ToString("f1");
            }

            yield return null;
        }

        if (s.isAvailable)
        {
            s.skillIcon.color = Color.white;
        }
        
        s.isOffCooldown = true;
    }
}

[System.Serializable]
public class UISkill
{
    [HideInInspector]
    public float cooldown;
    public Image skillIcon;
    public Text cooldownText;
    [HideInInspector]
    public float cooldownLeft;
    [HideInInspector]
    public bool isOffCooldown = true;
    [HideInInspector]
    public bool isAvailable = true;
    [HideInInspector]
    public Color skillUnavailableColor = Color.gray + (Color.white * 0.15f);

    public bool CanUseSkill()
    {
        return isOffCooldown && isAvailable;
    }

    public void SetUncastable()
    {
        isAvailable = false;
        skillIcon.color = skillUnavailableColor;
    }

    public void SetCastable(PhotonView photonView)
    {
        isAvailable = true;
        if (photonView.isMine && isOffCooldown)
        {
            skillIcon.color = Color.white;
        }
    }
}
