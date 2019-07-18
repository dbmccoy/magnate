using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

[RequireComponent(typeof(DeveloperSensor))]
public class DevelopAction : GoapAction {

    Person person;
    Entity Entity;
    DeveloperSensor sensor;

    public Project Project;
    public Building Building;
    public List<Tuple<Use,float>> Uses = new List<Tuple<Use, float>>();

    public DevelopProjectEvent OnCreateProject;

    public void Start() {
        person = GetComponent<Person>();
    }

    UseReqs reqs;
    LotMap map;
    List<IAsset> lots = new List<IAsset>();
    List<IAsset> blacklist = new List<IAsset>();

    bool meetsReqs = false;

    BuildingDesign design;
    Project designProj;
    Building building;
    Project bldProj;

    CommissionProjectAction commission;

    public override bool checkProceduralPrecondition(GameObject agent) {

        Entity = person.CurrentUnit.Entity;

        if(commission == null) {
            commission = GetComponent<CommissionProjectAction>();
        }

        if(reqs == null) {
            foreach (var g in person.FindGoals("developUseSqft")) {
                if(g.Value is UseReqs r) {
                    reqs = r;
                    map = reqs.map;
                    lots = map.Select(x => (IAsset)x.lot).ToList();
                    addEffect("developUseSqft", reqs);

                    SetLot((Lot)lots.FirstOrDefault());
                    break;
                }
            }

            if(reqs == null) {
                return false;
            }
        }

        if (lot != null) {
            addPrecondition(person.CurrentEntity.ID + "hasAsset", lot);
            addPrecondition("hasBldDesign", true);
            addPrecondition("hasBld", true);
            
            if (designProj == null) {
                commission.ClearProjectQueue();
                design = new BuildingDesign(Entity, reqs.use, reqs.sqft, lot);
                designProj = design.GetProject();
                designProj.Deliverable.Name = "des";
                commission.EnqueueProject(designProj);
                Debug.Log("adding design proj");
            }
            
            if (bldProj == null) {
                building = design.GetBuilding();
                bldProj = building.CreateProject();
                bldProj.Deliverable.Name = "bld";

                commission.EnqueueProject(bldProj);
                Debug.Log("adding bldg proj");
            }

        }
        else return false;

        return true;
    }

    public override bool isDone() {
        var done = meetsReqs || design.isComplete; //(bldProj != null && bldProj.isComplete());
        if (done) {
            person.RemoveGoal("developUseSqft", reqs);
            lot = null;
            Debug.Log("done developing");
            Preconditions.Clear();
            Effects.Clear();
            Project = null;
            bldProj = null;
            designProj = null;
            reqs = null;
            inProgress = false;
        }
        return done;
    }

    Lot lot;

    public void SetLot(Lot l) {
        Debug.Log("setting " + l.Address);
        lot = l;
    }

    bool inProgress;
    

    public override bool perform(GameObject agent) {

        if (meetsReqs) return true;

               
        return true;
    }

    public override bool requiresInRange() {
        return false;
    }

    public override void reset() {

    }
}

[System.Serializable]
public class DevelopProjectEvent : UnityEvent<Project> {
}
