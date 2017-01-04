using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;

    [SerializeField]
    private List<UISkill> skills;

    private InputManager inputManager;
    private List<PlayerSkill> playerSkills;

    private void Start()
    {
        inputManager = player.GetComponent<InputManager>();
        inputManager.OnPressedSkill += ActivateSkill;
        
        playerSkills = player.GetComponent<PlayerMovement>().skills;
        for(int i = 0; i < skills.Count; i++)
        {
            skills[i].cooldown = playerSkills[i].cooldown;
            skills[i].skillIcon.sprite = playerSkills[i].skillImage;
            skills[i].skillIcon.transform.parent.gameObject.GetComponent<Image>().sprite = playerSkills[i].skillImage;
        }
    }

    private void ActivateSkill(int skillId, Vector3 mousePosition)
    {
        if (skills[skillId].canUseSkill && playerSkills[skillId].CanUseSkill(mousePosition))
        {
            skills[skillId].canUseSkill = false;
            playerSkills[skillId].ActivateSkill();
            skills[skillId].cooldownLeft = skills[skillId].cooldown;
            skills[skillId].skillIcon.fillAmount = 0;
            StartCoroutine(SetSkillOffCooldown(skills[skillId]));
        }
    }

    private IEnumerator SetSkillOffCooldown(UISkill s)
    {
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
