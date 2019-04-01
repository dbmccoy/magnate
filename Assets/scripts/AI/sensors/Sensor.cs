using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Sensor : MonoBehaviour, ITemporal {

    protected GoapAgent Agent;
    protected Entity Entity;
    protected Person person;

    protected virtual void Awake()
    {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
        person.AddSensor(this);
        AddTemporal();
    }

    public abstract void Sense();

    public abstract HashSet<KeyValuePair<string, object>> ReturnWorldData();

   
    //asset, timestamp, value
    public List<Tuple<IAsset, float, float>> Evaluations;

    public abstract float EvaluateAsset(IAsset asset);

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





