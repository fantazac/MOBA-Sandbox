using UnityEngine;
using System.Collections;

public class MouseHover : MonoBehaviour
{
    private PlayerMouseSelection playerMouseSelection;

    private void OnMouseEnter()
    {
        if (StaticObjects.Player != null)
        {
            if (playerMouseSelection == null)
            {
                playerMouseSelection = StaticObjects.Player.PlayerMouseSelection;
            }
            playerMouseSelection.SetHoveredObject(gameObject);
        }
    }

    private void OnMouseExit()
    {
        if (StaticObjects.Player != null)
        {
            playerMouseSelection.UnhoverObject(gameObject);
        }
    }
}
