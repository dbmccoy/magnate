using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommissionProjectAction : GoapAction, IProjectAction {

    private Person person;
    private GoapAgent Agent;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    public void Awake()
    {
        person = GetComponent<Person>();
        WorkUnit = person.CurrentUnit; //TODO: fix
        Agent = GetComponent<GoapAgent>();
    }

    public Project Project { get; set; }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Entity = person.CurrentUnit.Entity;
        if(person.Project != null)
        {
            Project = person.Project;
            addEffect(Entity.ID+"hasAsset", Project.Deliverable);
            return true;
        }
        return false;
    }

    public override bool isDone()
    {
        var done = Entity.Assets.Contains(Project.Deliverable as IOwnable);
        if (done) {
            Debug.Log("done");
            person.RemoveGoal(Entity.ID + "hasAsset", Project.Deliverable);
        }
        return done;
    }

    bool isPosted = false;

    public override bool perform(GameObject agent)
    {
        if (!isPosted)
        {
            isPosted = true;
            ProjectBullitin.Instance.Add(Project);
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
        //removeEffect("hasAsset");
        //Project = null;
    }       

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
