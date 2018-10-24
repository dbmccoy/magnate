using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;


public class TestGoal : ReGoapGoal<string,object> {

    protected override void Awake()
    {
        base.Awake();
        Name = "HaveJob";
        goal.Set("hasJob", true);
    }
}
