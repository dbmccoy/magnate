using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObj : Obj {

	public Building Building { get; private set; }
    public Lot Lot;

    public void Init(Building building)
    {
        Building = building;
        Lot = building.Lot;
        transform.position = Lot.center;
    }

    public void Update()
    {
        transform.position = Lot.center + new Vector3(0, -2 + (Building.project.PercentComplete) * 2, 0);
    }
}
