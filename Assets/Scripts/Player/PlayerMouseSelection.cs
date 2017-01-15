using UnityEngine;
using System.Collections;

public class PlayerMouseSelection : PlayerBase
{
    private GameObject hoveredObject;
    private GameObject clickedObject;

    private EntityTeam hoveredObjectTeam;
    private EntityTeam clickedObjectTeam;

    public void SetHoveredObject(GameObject hoveredObject)
    {
        hoveredObjectTeam = hoveredObject.GetComponent<EntityTeam>();
        if(hoveredObjectTeam != null)
        {
            this.hoveredObject = hoveredObject;
            //Debug.Log("Hovered " + hoveredObject.name);
        }
    }

    public void UnhoverObject(GameObject unhoveredObject)
    {
        if(unhoveredObject == hoveredObject)
        {
            hoveredObject = null;
            hoveredObjectTeam = null;
            //Debug.Log("Unhovered " + unhoveredObject.name);
        }
    }

    public bool HoveredObjectIsAlly(Team objectTeam)
    {
        return hoveredObjectTeam.Team == objectTeam;
    }

    public bool HoveredObjectIsEnemy(Team objectTeam)
    {
        return hoveredObjectTeam.Team != objectTeam;
    }


}
