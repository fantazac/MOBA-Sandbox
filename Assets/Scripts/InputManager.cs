using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public delegate void OnPressedSkillHandler(int skillId, Vector3 mousePosition);
    public event OnPressedSkillHandler OnPressedSkill;

    public delegate void OnPressedYHandler();
    public event OnPressedYHandler OnPressedY;

    public delegate void OnPressedSpaceHandler();
    public event OnPressedSpaceHandler OnPressedSpace;

    public delegate void OnReleasedSpaceHandler();
    public event OnReleasedSpaceHandler OnReleasedSpace;

    public delegate void OnRightClickHandler(Vector3 mousePosition);
    public event OnRightClickHandler OnRightClick;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnPressedSkill(0, Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnPressedSkill(1, Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            OnPressedY();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPressedSpace();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnReleasedSpace();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick(Input.mousePosition);
        }
    }
}
