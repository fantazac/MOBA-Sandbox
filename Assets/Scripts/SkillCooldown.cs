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

            yield return null;
        }
        s.canUseSkill = true;
    }
}

[System.Serializable]
public class UISkill
{
    public float cooldown;
    public Image skillIcon;
    [HideInInspector]
    public float cooldownLeft;
    [HideInInspector]
    public bool canUseSkill = true;

}
