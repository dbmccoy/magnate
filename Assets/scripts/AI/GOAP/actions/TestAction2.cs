using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class TestAction2 : ReGoapAction<string,object> {

    protected override void Awake()
    {
        base.Awake();
        Name = "GetHouse";
        preconditions.Set("hasJob", true);
        effects.Set("hasHouse", true);
    }

    public void OnDisable()
    {

    }

    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings, ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        GetComponent<Person>().Residence = "House";
        Debug.Log("has house");
        doneCallback(this);
    }
}
