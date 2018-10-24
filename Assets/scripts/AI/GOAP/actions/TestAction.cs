using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;
using ReGoap.Core;
using System;

public class TestAction : ReGoapAction<string,object> {

    Person Person;

    protected override void Awake()
    {
        base.Awake();
        Name = "GetJob";
        effects.Set("hasJob", true);
    }

    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next
        , ReGoapState<string, object> settings, ReGoapState<string, object> goalState
        , Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        Person = GetComponent<Person>();
        Debug.Log("is mailman");
        Person.Job = "Mailman";
        doneCallback(this);
    }
}
