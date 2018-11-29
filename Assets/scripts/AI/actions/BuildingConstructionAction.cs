using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingConstructionAction : GoapAction, IProjectAction
{
    private Person person;
    private GoapAgent Agent;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    public Project Project { get; set; }
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
        Agent = GetComponent<GoapAgent>();
    }

    public override void reset()
    {
        Project = null;
        try
        {
            removeEffect(Entity.ID + "hasAsset");
        }
        catch { }
        try
        {
            removePrecondition("meetsWorkReqs");
        }
        catch { }
    }

    public override bool isDone()
    {
        return Project.isComplete();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        bool ok = false;

        if (person.Project != null && person.Project.Deliverable is Building b)
        {
            Project = person.Project;
            var OwnerEntity = Project.Entity;

            building = b;

            addPrecondition("meetsWorkReqs", Project);
            addEffect(OwnerEntity.ID+"hasAsset", b);
            ok = true;
        }
        return ok;
    }

    private bool inProgress = false;

    public override bool perform(GameObject agent)
    {
        if (inProgress == false)
        {
            inProgress = true;

            building.StartConstruction();
            person.CurrentUnit.AddProject(Project);
        }

        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }
}


