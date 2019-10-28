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
        sensor = GetComponent<DeveloperSensor>();
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
        addPrecondition("hasBld", true);


        if (commission == null) {
            commission = GetComponent<CommissionProjectAction>();
        }

        if(lot == null) {
            foreach (var g in person.FindGoals("developUseSqft")) {
                if(g.Value is UseReqs r) {
                    reqs = r;
                    map = reqs.map;
                    lots = map.Select(x => (IAsset)x.lot).ToList();
                    addEffect("developUseSqft", reqs);

                    SetLot((Lot)lots.Where(x => !blacklist.Contains(x)).FirstOrDefault());
                    break;
                }
            }

            if (lot == null) {
                return failProceduralPreconditions();
            }
        }

        if (lot != null) {
            addPrecondition(person.CurrentEntity.ID + "hasAsset", lot);

            if(lot.Building != null) {
                return failProceduralPreconditions();
            }
            
            if (designProj == null) {
                design = new BuildingDesign(Entity, reqs.use, reqs.sqft, lot);
                designProj = design.GetProject();
                designProj.Deliverable.Name = design.lot.Address + " design";
                person.PlanningProjects.Add(designProj);
            }
            
            if (bldProj == null) {
                building = design.GetBuilding();
                bldProj = building.CreateProject();
                bldProj.Deliverable.Name = building.Lot.Address + " bld";
                person.PlanningProjects.Add(bldProj);
                //commission.EnqueueProject(bldProj);
            }

        }
        else return failProceduralPreconditions();

        return true;
    }

    protected override bool failProceduralPreconditions() {
        if(lot == null) {
            return false;
        }
        blackListCount++;
        if(blackListCount >= blackListLimit) {
            blackListCount = 0;
            blacklist.Add(lot);
            Debug.Log("adding " + lot.Address + " to blacklist");
            hardReset();
        }
        return false;
    }

    public override bool isDone() {

        if(bldProj != null && bldProj.isComplete()) {
            meetsReqs = true;
        }

        var done = meetsReqs || design.isComplete;

        Debug.Log(person + ": develop action complete: " + done);

        if (done) {
            person.RemoveGoal("developUseSqft", reqs);
            //sensor.CompleteProject();
            Debug.Log("DevelopAction.isDone: complete");
            blacklist.Add(lot);
            Debug.Log("adding " + lot.Address + " to blacklist");
            hardReset();
        }
        return done;
    }

    Lot lot;

    public void SetLot(Lot l) {
        Debug.Log("setting " + l.Address);
        lot = l;
    }

    public void BlacklistLot(Lot l) {

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

    protected override void hardReset() {
        lot = null;
        Preconditions.Clear();
        design = null;
        building = null;
        Effects.Clear();
        Project = null;
        bldProj = null;
        designProj = null;
        reqs = null;
        map = null;
        inProgress = false;
    }
}

[System.Serializable]
public class DevelopProjectEvent : UnityEvent<Project> {
}
