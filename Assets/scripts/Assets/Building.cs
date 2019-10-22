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
    public BuildingDesign Design { get; private set; }

    public List<System.Tuple<Use, float>> Uses = new List<System.Tuple<Use, float>>();

    public BuildingObj BuildingObj { get; private set; }
    public float PercentComplete { get; private set; }
    public bool isComplete { get; private set; }

    public Neighborhood Neighborhood { get; set; }
    public string Class { get; set; }
    public float LastSalePrice { get; set; }
    public float ValueToOwner { get; set; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public Building(BuildingDesign design, bool iscomplete = false)
    {
        OwningEntity = design.OwningEntity;
        SetDesign(design);
        Lot = design.lot;
        Floors = design.floors;
        SquareFeet = design.sqft;
        isComplete = iscomplete;
        Class = "RealEstate";
        Neighborhood = Lot.Neighborhood;
    }

    public void SetDesign(BuildingDesign d) {
        Design = d;
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
        project.prereqs.Add("hasBldDesign", true);
        project.effects.Add("hasBld", true);

        //project.prereqs.Add(Lot.Address + "isBuildable", true);

        return project;
    }

    public void StartConstruction()
    {
        Debug.Log("Starting construction " + Lot.Address);
        GameObject obj = GameObject.Instantiate(Resources.Load("buildings/house_test_1"), Lot.transform) as GameObject;
        BuildingObj = obj.GetComponent<BuildingObj>();
        Lot.Building = this;
        BuildingObj.Init(this);
    }


    public void Complete()
    {
        PercentComplete = 100f;
        Transfer(OwningEntity);
        project.ForceComplete();
        Lot.OnLotUpdate.Invoke(Lot); //TODO: move this to lot
        Debug.Log("bld done");
        
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


public class BuildingDesign : IProjectable {
    public string Name { get; set; }

    private Use use;
    public int sqft;
    public Lot lot;
    public int floors;

    public bool isComplete;
    public Building Building;
    public Project BuildingProject;
    private Project project;
    public Entity OwningEntity { get; set; }

    public Project GetProject() {
        if (project != null) {
            return project;
        }
        else {
            project = CreateProject();
            return project;
        }
    }

    public Project CreateProject() {
        var Reqs = new List<WorkReq>
        {
            new WorkReq(SkillType.BldDesign, sqft)
        };

        project = new Project(OwningEntity, this, Reqs);

        project.prereqs.Add(project.Deliverable.OwningEntity.ID + "hasAsset", lot);
        //project.prereqs.Add("hasLotForDev", true);
        project.effects.Add("hasBldDesign", true);
        //project.prereqs.Add(lot.Address + "isBuildable", true);

        return project;
    }

    public BuildingDesign(Entity e, Use u, int s, Lot l, int fl = 0) {
        OwningEntity = e;
        use = u;
        sqft = s;
        lot = l;
        floors = 1; //TODO:
        Name = l.Address + " design";
    }

    public Building GetBuilding() {
        if (Building != null) {
            return Building;
        }
        else {
            Building = new Building(this);
            return Building;
        }
    }

    public void Complete() {
        isComplete = true;
        project.ForceComplete();
        lot.SetDesign(this);
    }

}


/*
public static class BuildingCreator {
    public static Building CreateBuilding(Entity owner, int fls, int sqf, Lot lot = null) {
        Building b = new Building(owner, fls, sqf, lot);
        return b;
    }
}
*/
