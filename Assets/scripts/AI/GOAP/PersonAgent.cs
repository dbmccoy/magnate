using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity;

public class PersonAgent : ReGoapAgentAdvanced<string, object>
{
    public Person Person;

    protected override void Awake()
    {
        base.Awake();
        Person = GetComponent<Person>();
    }
}
