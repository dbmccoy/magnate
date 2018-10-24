using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class BuildingConstructionAction : ReGoapAction<string, object>
{
    private Person Person;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    private Project project;
    private Building building;
    

    protected override void Awake()
    {
        base.Awake();
        Name = "BuildingConstruction";
        //WorkUnit = GetComponent<WorkUnit>();
        //Entity = GetComponent<Entity>();
        Person = GetComponent<Person>();
        //building = project.Deliverable as Building;
    }

    public void SetProject(Project p)
    {
        project = p;
        effects.Set("hasAsset", p.Deliverable as IOwnable);
    }

    public void SetPrecondition(string key, object value)
    {
        preconditions.Set(key, value);
    }

    public void SetEffect(string key, object value)
    {
        building = value as Building;
        Debug.Log(building.Name);
        effects.Set(key, value);
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        //Debug.Log(building.Name);
        return base.CheckProceduralCondition(stackData) && project != null;
    }

    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        Person.CurrentUnit.AddProject(project);
        //Person.Projects.Enqueue(project);
        //Person.CurrentUnit.AddProject(project);
    }

}


