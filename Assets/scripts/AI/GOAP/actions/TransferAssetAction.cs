using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Core;
using ReGoap.Unity;

public class TransferAssetAction : ReGoapAction<string, object> {

    public IOwnable Asset;

    protected override void Awake()
    {
        base.Awake();
        preconditions.Set("hasAsset", Asset);
        effects.Set("myEffects", true);
    }
    public override void Run(IReGoapAction<string, object> previous, IReGoapAction<string, object> next, ReGoapState<string, object> settings,
        ReGoapState<string, object> goalState, Action<IReGoapAction<string, object>> done, Action<IReGoapAction<string, object>> fail)
    {
        base.Run(previous, next, settings, goalState, done, fail);
        // do your own game logic here
        // when done, in this function or outside this function, call the done or fail callback, automatically saved to doneCallback and failCallback by ReGoapAction
        doneCallback(this); // this will tell the ReGoapAgent that the action is succerfully done and go ahead in the action plan
        // if the action has failed then run failCallback(this), the ReGoapAgent will automatically invalidate the whole plan and ask the ReGoapPlannerManager to create a new plan
    }
}
