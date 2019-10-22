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

    public Project tempProject { get; set; }
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
        try
        {
            removeEffect(Entity.ID + "hasAsset");
            //Project = null;
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
        var c = tempProject.isComplete();
        if (c) {
            hardReset();
        }
        return c;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        if (person.Project != null && 
            person.Project.Deliverable is Building b
            )
        {
            tempProject = person.Project;
            var OwnerEntity = tempProject.Entity;

            building = b;
            //addPrecondition("hasBldDesign", true);
            addPrecondition("meetsWorkReqs", tempProject);
            addEffect(OwnerEntity.ID+"hasAsset", b.Name);
            //addEffect(b.Lot + "hasBuilding", b.Design);
            addEffect(tempProject + "complete", true);

            return true;
        }
        return failProceduralPreconditions();
    }

    protected override bool failProceduralPreconditions() {
        blackListCount++;
        if(blackListCount >= blackListLimit) {
            hardReset();
        }
        return false;
    }

    protected override void hardReset() {
        blackListCount = 0;
        person.Project = null;
        building = null;
        Preconditions.Clear();
        Effects.Clear();
        if(tempProject != null) {
            person.RemoveGoal(tempProject.Entity.ID + "hasAsset", tempProject.Deliverable.Name);
        }
        
        tempProject = null;
        inProgress = false;
    }

    private bool inProgress = false;

    public override bool perform(GameObject agent)
    {
        if (inProgress == false)
        {
            inProgress = true;

            Debug.Log(building.Name + " BEGIN");
            building.StartConstruction();
            person.CurrentUnit.AddProject(tempProject);
        }

        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }
}

