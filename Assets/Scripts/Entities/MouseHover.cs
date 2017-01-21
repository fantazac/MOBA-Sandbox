using UnityEngine;
using System.Collections;

public class MouseHover : MonoBehaviour
{
    private PlayerMouseSelection playerMouseSelection;

    private void OnMouseEnter()
    {
        if(playerMouseSelection == null)
        {
            playerMouseSelection = StaticObjects.Player.PlayerMouseSelection;
        }
        playerMouseSelection.SetHoveredObject(gameObject);
    }

    private void OnMouseExit()
    {
        playerMouseSelection.UnhoverObject(gameObject);
    }

}
