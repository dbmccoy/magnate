using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor : MonoBehaviour, ITemporal {

    protected GoapAgent Agent;
    protected Person person;

    protected virtual void Awake()
    {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
        AddTemporal();
    }

    public abstract void Sense();

    public virtual void AddTemporal()
    {
        var temp = new Temporal(this);
    }

    public virtual void DayTick()
    {
        Sense();
    }

    public virtual void MonthTick()
    {
    }
}
