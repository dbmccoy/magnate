using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class TestGoal2 : ReGoapGoal<string,object> {

    protected override void Awake()
    {
        base.Awake();
        Name = "haveHouse";
        goal.Set("hasHouse", true);
    }
}
