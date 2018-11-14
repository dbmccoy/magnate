using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    public Entity Entity;
    public WorkUnit Unit;
    public List<Skill> Skills;
    public Person Manager;
    public float Pay;

    public Job(Entity entity, WorkUnit unit, List<Skill> skills, Person manager)
    {
        Entity = entity;
        Unit = unit;
        Skills = skills;
        Manager = manager;
    }

    //Location

    //Pay (can be negative for dues paying orgs)

    //Hours
}
