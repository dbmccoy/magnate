using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class DevelopAction : GoapAction {

    Person person;
    Entity Entity;
    DeveloperSensor sensor;

    public Project Project;
    public Building Building;
    public List<Tuple<Use,float>> Uses = new List<Tuple<Use, float>>();

    public DevelopProjectEvent OnCreateProject;

    public DevelopAction() {
    }

    public void Start() {
        person = GetComponent<Person>();
    }

    public override bool checkProceduralPrecondition(GameObject agent) {

        if(sensor == null) {
            sensor = GetComponent<DeveloperSensor>();
        }
        addEffect("hasProject", true);
        Entity = person.CurrentUnit.Entity;

        List<Tuple<Use, float>> l = new List<Tuple<Use, float>>();
        foreach (var g in person.FindGoals("developUseSqft")) {
            l.Add((Tuple<Use,float>)g.Value);
        }

        if (Project == null && l.Count > 0) {
            CreateProject(l);
        }

        return true;

        if (sensor.Priority != null) {
            //addEffect("develop" + sensor.Priority.Name, true);
            return true;
        }

        return false;
    }

    public override bool isDone() {
        var done = person.Project == Project && Project != null;
        if (done) {
            person.AddGoal(Entity.ID + "hasAsset", Project.Deliverable.Name);
            //person.RemoveGoal("develop" + sensor.Priority.Name, true);
            lot = null;
            Debug.Log("done developing");
            Project = null;
            inProgress = false;
        }
        return done;
    }

    Lot lot;

    public void SetLot(Lot l) {
        lot = l;
    }

    void CreateProject(List<Tuple<Use,float>> l) {

        if (lot == null) {
            Debug.Log("couldn't find lot");
            return;
        }

        Building building = new Building(person.Name + UnityEngine.Random.Range(0, 1000).ToString(), Entity, lot, 2, 10);

        foreach(var use in Uses) {
            building.AddUse(use);
        }
        
        Building = building;

        Project = building.CreateProject();
        person.PlanningProject = Project;

    }

    bool inProgress;

    public override bool perform(GameObject agent) {

        if (!inProgress) {
            inProgress = true;
            sensor.AddProject(Project);
            person.AddProject(Project);
        }
        
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
