using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingConstructionAction : GoapAction
{
    private Person person;
    private GoapAgent agent;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    private Project project;
    [SerializeField]
    public Building building;

    public static HashSet<SkillType> SkillReqs = new HashSet<SkillType>
    {
          SkillType.BldFoundation
        , SkillType.BldFraming
        , SkillType.BldFinishing
    };

    public void Awake()
    {
        person = GetComponent<Person>();
        agent = GetComponent<GoapAgent>();
    }

    public void AddProject(Project p)
    {
        project = p;
        building = p.Deliverable as Building; //test for this

        addPrecondition("meetsWorkReqs", project);
        addEffect("hasAsset", p.Deliverable as Building);
    }

    public override void reset()
    {
    }

    public override bool isDone()
    {
        return project.isComplete();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return project != null;
    }

    private bool inProgress = false;

    public override bool perform(GameObject agent)
    {
        if (inProgress == false)
        {
            inProgress = true;

            building.StartConstruction();
            person.CurrentUnit.AddProject(project);
        }

        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }
}


