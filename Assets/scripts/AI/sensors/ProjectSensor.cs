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

    }

    public void EnqueProject(Project project)
    {
        //bid process goes where?
        if (CanDeliver(project))
        {
            person.AddProject(project);
        }
    }

    public bool CanDeliver(Project project)
    {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
        var state = Agent.dataProvider.getWorldState();
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add(new KeyValuePair<string, object>(project.Entity.ID+"hasAsset", project.Deliverable));

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
}
