using System.Collections;
using System.Collections.Generic;
using ReGoap.Unity;
using UnityEngine;

[RequireComponent(typeof(PersonAgent))]
[RequireComponent(typeof(PersonMemory))]

public class Person : MonoBehaviour, IProductive{

    public string Name;
    public string Residence; //convert to unit
    public string Job; //convert to Job
    public E_Race Race;
    PersonAgent agent;
    PersonMemory memory;
    public Entity Entity;
    public Queue<Project> Projects = new Queue<Project>();

    public void Awake()
    {
        Name = "Jeff";
        Entity = new Entity(" Entity");
        Skills = new Dictionary<Work.Type, float>();
        agent = GetComponent<PersonAgent>();
        agent.Name = Name;
        memory = GetComponent<PersonMemory>();
        AddTemporal();
    }

    public PersonAgent GetAgent()
    {
        return agent;
    }

    //IProductive
    public WorkUnit CurrentUnit { get; set; }
    public Project CurrentProject { get; set; }
    public Dictionary<Work.Type, float> Skills { get; set; }
    public float Capacity { get; set; }

    public void AssignUnit(WorkUnit unit)
    {
        unit.AddInput(this);
        CurrentUnit = unit;
    }

    public void AddSkill(Work.Type type, float val)
    {
        if(Skills == null)
        {
            Skills = new Dictionary<Work.Type, float>();
        }
        Skills.Add(type, val);
    }

    public void AssignProject(Project project)
    {
        CurrentProject = project;
    }

    public void DoWork()
    {
        CurrentUnit.TakeInput(this);
        Capacity--;
    }

    public void AddTemporal()
    {
        new Temporal(this);
    }

    public void DayTick()
    {
        if (CurrentUnit != null)
        {
            DoWork();
        }
        else
        {
            Debug.Log(Name + " no work");
        }
    }

    public void MonthTick()
    {
    }
    
    public enum E_Race
    {
        white,
        black,
        latino,
        asian
    }
}

