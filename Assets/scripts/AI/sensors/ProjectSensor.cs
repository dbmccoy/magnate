using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ProjectSensor : Sensor {

    protected override void Awake()
    {
        base.Awake();
        ProjectBullitin.Instance.AddProjectToBullitinEvent.AddListener(EnqueProject);
    }

    public override void Sense()
    {
        if(person.Project == null) {
            var p = ProjectBullitin.Instance.Available.Random();
            if(p != null) {
                EnqueProject(p);
            }
        }
    }

    public override List<LotMap> GetLotMaps() {
        return null;
    }

    public override HashSet<KeyValuePair<string, object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        return data;
    }


    public void EnqueProject(Project project)
    {
        Debug.Log(person.name + " senses project " + project.Deliverable.ToString() + " for " + project.Deliverable.OwningEntity.Officer);
        //bid process goes where?
        if (CanDeliver(project)) {
            Debug.Log(person.Name + ": I can deliver this");
            person.AddProject(project);
        }
        else Debug.Log(person.Name + " fail");
    }

    public bool CanDeliver(Project project)
    {
        if(person.Project != null) {
            return false;
        }
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
        var state = Agent.dataProvider.getWorldState();
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        //goal.Add(new KeyValuePair<string, object>(project.Entity.ID+"hasAsset", project.Deliverable.Name));

        goal.Add(new KeyValuePair<string, object>(project + "complete", true));

       /* if (project.Deliverable is BuildingDesign bd) {
            goal.Add(new KeyValuePair<string, object>(bd.lot + "hasDesign", true));
        }*/
        

        var dummy = new GameObject("dummy");
        var actions = Agent.GetComponents(typeof(GoapAction));
        var dummyPerson = (Person)dummy.AddComponent(typeof(Person));
        dummyPerson.isDummy = true;
        dummyPerson.Project = project;
        var dummyActions = new HashSet<GoapAction>();
        foreach (var action in actions)
        {
            var dummyAct = dummy.AddComponent(action.GetType());
            dummyActions.Add((GoapAction)dummyAct);
        }

        var plan = Agent.planner.plan(Agent.gameObject, dummyActions, state, goal);

        Destroy(dummy);

        return plan != null;
    }

    public override float EvaluateAsset(IAsset asset) {
        throw new System.NotImplementedException();
    }
}
