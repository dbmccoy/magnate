using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
public class TransferAssetAction : GoapAction {
    private Person person;
    private GoapAgent Agent;

    public Entity Entity { get; set; }

    public Project Project { get; set; }
    public Entity TransferTo;

    public void Awake()
    {
        person = GetComponent<Person>();
        Agent = GetComponent<GoapAgent>();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Entity = person.CurrentUnit.Entity;
        Project = person.Project;
        if(Project != null && TransferTo != null)
        {
            addPrecondition(Entity.ID + "hasAsset", Project.Deliverable);
            addEffect(TransferTo.ID + "hasAsset", Project.Deliverable);
            return true;
        }
        return false;
    }

    public override bool isDone()
    {
        return TransferTo.Assets.Contains(Project.Deliverable as IAsset);
    }

    public override bool perform(GameObject agent)
    {
        if(Entity.Assets.Contains(Project.Deliverable as IAsset))
        {
            TransferTo.AcquireAsset(Project.Deliverable as IAsset);
            Entity.DivestAsset(Project.Deliverable as IAsset);
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
        try
        {
            removePrecondition(Entity.ID + "hasAsset");
        }
        catch { }
        try
        {
            removeEffect(TransferTo.ID + "hasAsset");
        }
        catch { }
    }
}
