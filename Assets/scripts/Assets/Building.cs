using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class Building : IBuilding {
    public string Name { get; set; }
    public Entity OwningEntity { get; set; }
    public int Floors { get; private set; }
    public int SquareFeet { get; private set; }
    public Lot Lot { get; private set; }

    public List<System.Tuple<Use, float>> Uses = new List<System.Tuple<Use, float>>();

    public BuildingObj BuildingObj { get; private set; }
    public float PercentComplete { get; private set; }
    public bool isComplete { get; private set; }

    public Neighborhood Neighborhood { get; set; }
    public string Class { get; set; }
    public float LastSalePrice { get; set; }
    public float ValueToOwner { get; set; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public Building(Entity owner, int fls, int sqf, Lot lot = null, bool iscomplete = false)
    {
        OwningEntity = owner;
        Lot = lot;
        Floors = fls;
        SquareFeet = sqf;
        isComplete = iscomplete;
        Class = "RealEstate";
        Neighborhood = Lot.Neighborhood;
    }

    public void AddUse(System.Tuple<Use,float> use) {

    }

    public Project project;

    public Project CreateProject()
    {
        var Reqs = new List<WorkReq>
        {
            new WorkReq(SkillType.BldFoundation, SquareFeet, 1, order: 1, maxAmt: SquareFeet), //needs formula
            new WorkReq(SkillType.BldFraming, SquareFeet, 1, order: 2, maxAmt: SquareFeet),
            new WorkReq(SkillType.BldFinishing, SquareFeet, 1, order: 3, maxAmt: SquareFeet)
        };
        project = new Project(OwningEntity, this, Reqs);
        project.prereqs.Add(OwningEntity.ID+"hasAsset", Lot);

        project.prereqs.Add(Lot.Address + "isBuildable", true);

        return project;
    }

    public void StartConstruction()
    {
        Debug.Log("Starting construction " + Name);
        GameObject obj = GameObject.Instantiate(Resources.Load("buildings/house_test_1"), Lot.transform) as GameObject;
        BuildingObj = obj.GetComponent<BuildingObj>();
        Lot.Buildings.Add(this);
        BuildingObj.Init(this);
    }


    public void Complete()
    {
        PercentComplete = 100f;
        Transfer(OwningEntity);
        
        isComplete = true;
    }

    public float GetValue() {
        throw new NotImplementedException();
    }

    public float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }

    public void GrantActionsTo(Person p) {

    }

    public void RevokeActionsFrom(Person p) {
    }

    public void Transfer(Entity to) {
        OwningEntity = to;
        OwningEntity.AcquireAsset(this);
        foreach (var u in Uses) {
            //u.Assets.ForEach(x => OwningEntity.AcquireAsset(x));
        }
    }
}

public static class BuildingCreator {
    public static Building CreateBuilding(Entity owner, int fls, int sqf, Lot lot = null) {
        Building b = new Building(owner, fls, sqf, lot);
        return b;
    }
}

