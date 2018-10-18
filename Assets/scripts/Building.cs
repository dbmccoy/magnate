using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : IBuilding {
    public string Name { get; set; }
    public Entity OwningEntity { get; set; }
    public int Floors { get; private set; }
    public int SquareFeet { get; private set; }
    public Lot Lot { get; private set; }

    public BuildingObj BuildingObj { get; private set; }
    public float PercentComplete { get; private set; }
    public bool isComplete { get; private set; }

    public Building(string name, Entity owner, Lot lot, int fls, int sqf, bool iscomplete = false)
    {
        Name = name;
        OwningEntity = owner;
        Lot = lot;
        Floors = fls;
        SquareFeet = sqf;
        isComplete = iscomplete;
    }

    public Project CreateProject()
    {
        var Reqs = new List<WorkReq>();
        Reqs.Add(new WorkReq(Work.Type.BldFoundation, SquareFeet, 1, order: 1, maxAmt: SquareFeet)); //needs formula
        Reqs.Add(new WorkReq(Work.Type.BldFraming, SquareFeet, 1, order: 2, maxAmt: SquareFeet));
        Reqs.Add(new WorkReq(Work.Type.BldFinishing, SquareFeet, 1, order: 3, maxAmt: SquareFeet));
        return new Project(this, Reqs);
    }

    public void StartConstruction()
    {
        BuildingObj = MonoBehaviour.Instantiate(BuildingObj, Lot.transform);
        BuildingObj.transform.position = Lot.transform.position;

        ////bootstrap
        CompleteConstruction();

    }

    public void CompleteConstruction()
    {
        PercentComplete = 100f;
        isComplete = true;
    }

    public float GetValue()
    {
        throw new System.NotImplementedException();
    }

    public void Transfer(Entity to)
    {
        throw new System.NotImplementedException();
    }
}
