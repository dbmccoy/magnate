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

    public Project project;

    public Project CreateProject()
    {
        var Reqs = new List<WorkReq>();
        Reqs.Add(new WorkReq(SkillType.BldFoundation, SquareFeet, 1, order: 1, maxAmt: SquareFeet)); //needs formula
        Reqs.Add(new WorkReq(SkillType.BldFraming, SquareFeet, 1, order: 2, maxAmt: SquareFeet));
        Reqs.Add(new WorkReq(SkillType.BldFinishing, SquareFeet, 1, order: 3, maxAmt: SquareFeet));
        project = new Project(this, Reqs);
        return project;
    }

    public void StartConstruction()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("buildings/house_test_1"), Lot.transform) as GameObject;
        BuildingObj = obj.GetComponent<BuildingObj>();
        BuildingObj.Init(this);
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
