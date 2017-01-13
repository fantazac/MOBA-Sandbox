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
        foreach (Transform child in transform.parent.parent.transform)
        {
            player = child.gameObject.GetComponent<Player>();
        }
        if (player != null)
        {
            player.PlayerInput.OnPressedSkill += ActivateSkill;

            playerSkills = player.skills;
            for (int i = 0; i < playerSkills.Count; i++)
            {
                if (playerSkills[i] != null)
                {
                    skills[i].cooldown = playerSkills[i].cooldown;
                    skills[i].skillIcon.sprite = playerSkills[i].skillImage;
                    skills[i].skillIcon.transform.parent.gameObject.GetComponent<Image>().sprite = playerSkills[i].skillImage;
                    if (playerSkills[i].canBeCancelled)
                    {
                        playerSkills[i].SetCooldown += SetCooldown;
                    }
                }
            }
        }
        else
        {
            throw new Exception("Player set to null reference");
        }
    }

    private void SetCooldown(int skillId)
    {
        skills[skillId].cooldownLeft = skills[skillId].cooldown;
        skills[skillId].skillIcon.fillAmount = 0;
        StartCoroutine(SetSkillOffCooldown(skills[skillId]));
    }

    private void ActivateSkill(int skillId, Vector3 mousePosition)
    {
        if (skills[skillId].canUseSkill && playerSkills[skillId].CanUseSkill(mousePosition))
        {
            skills[skillId].canUseSkill = false;
            playerSkills[skillId].ActivateSkill();
            if (playerSkills[skillId].cooldownStartsOnCast)
            {
                SetCooldown(skillId);
            }
        }
        else if (playerSkills[skillId].canBeCancelled && playerSkills[skillId].skillActive && !skills[skillId].canUseSkill)
        {
            playerSkills[skillId].CancelSkill();
        }
    }

    private IEnumerator SetSkillOffCooldown(UISkill s)
    {
        s.skillIcon.color = Color.gray + (Color.white * 0.15f);
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
        s.skillIcon.color = Color.white;
        s.canUseSkill = true;
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
    public bool canUseSkill = true;

}
