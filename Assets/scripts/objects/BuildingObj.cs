using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObj : Obj {

	public Building Building { get; private set; }

    public void SetBuilding(Building building)
    {
        Building = building;
    }

}
