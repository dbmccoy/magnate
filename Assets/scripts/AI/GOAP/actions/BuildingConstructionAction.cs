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
        //WorkUnit = GetComponent<WorkUnit>();
        //Entity = GetComponent<Entity>();
        Person = GetComponent<Person>();
        preconditions.Set("hasProject", Person.Projects);
        //building = project.Deliverable as Building;
        effects.Set("hasAsset", building as IOwnable);
    }

    public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
    {
        var  q = (Queue<Project>)preconditions.Get("hasProject");
        try
        {
            project = q.Dequeue();
        }
        catch
        {
            return false;
        }

        return base.CheckProceduralCondition(stackData) && project != null;
    }

    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {

        base.Run(previous, next, settings, goalState, done, fail);
    }

}


