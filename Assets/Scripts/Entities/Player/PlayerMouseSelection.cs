using UnityEngine;
using System.Collections;

public class PlayerMouseSelection : PlayerBase
{
    public GameObject HoveredObject { get; private set; }
    public GameObject ClickedObject { get; private set; }

    private EntityTeam hoveredObjectTeam;
    private EntityTeam clickedObjectTeam;

    public void SetHoveredObject(GameObject hoveredObject)
    {
        hoveredObjectTeam = hoveredObject.GetComponent<EntityTeam>();
        if(hoveredObjectTeam != null)
        {
            HoveredObject = hoveredObject;
            //Debug.Log("Hovered " + hoveredObject.name);
        }
    }

    public void UnhoverObject(GameObject unhoveredObject)
    {
        if(unhoveredObject == HoveredObject)
        {
            HoveredObject = null;
            hoveredObjectTeam = null;
            //Debug.Log("Unhovered " + unhoveredObject.name);
        }
    }

    public bool HoveredObjectIsAlly(Team objectTeam)
    {
        if(HoveredObject == null)
        {
            return false;
        }
        return hoveredObjectTeam.Team == objectTeam;
    }

    public bool HoveredObjectIsEnemy(Team objectTeam)
    {
        if (HoveredObject == null)
        {
            return false;
        }
        return hoveredObjectTeam.Team != objectTeam;
    }


}
