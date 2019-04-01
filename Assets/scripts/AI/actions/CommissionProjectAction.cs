using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommissionProjectAction : GoapAction, IProjectAction {

    private Person person;
    private GoapAgent Agent;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    DeveloperSensor devSensor;

    public void Awake()
    {
        person = GetComponent<Person>();
        WorkUnit = person.CurrentUnit; //TODO: fix
        Agent = GetComponent<GoapAgent>();
        devSensor = GetComponent<DeveloperSensor>();

        addPrecondition("hasProject", true);
    }

    public Project Project { get; set; }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Entity = person.CurrentUnit.Entity;

        if (devSensor.Priority != null) {
            addEffect("develop" + devSensor.Priority.Name, true);
        }

        if(Project == null) {
            Project = person.PlanningProject;
        }

        if(Project != null) {
            foreach (var i in Project.prereqs) {
                addPrecondition(i.Key, i.Value);
                //Debug.Log("adding precond " + i.Key + ":" + i.Value.ToString());
            }
            return true;
        }
        return false;

        if (person.Project != null)
        {
            
            //addEffect(Entity.ID+"hasAsset", Project.Deliverable);
            return true;
        }
    }

    public override bool isDone()
    {
        if(Project == null || Project.Deliverable == null) {
            return false;
        }
        var done = Entity.Assets.Contains(Project.Deliverable as IAsset);
        if (done) {
            person.RemoveGoal(Entity.ID + "hasAsset", Project.Deliverable.Name);
            Debug.Log("taking possession of " + Project.Deliverable.Name);
            person.Project = null;
            person.PlanningProject = null;

            //reset added preconditions
            foreach (var i in Project.prereqs) {
                removePrecondition(i.Key);
                Debug.Log("Removing precond " + i.Key + " " + i.Value);
            }

            Project = null;
            isPosted = false;
        }
        return done;
    }

    bool isPosted = false;

    public override bool perform(GameObject agent)
    {
        if (!isPosted)
        {
            Project = person.Project;
            isPosted = true;
            ProjectBullitin.Instance.Add(Project);
            Debug.Log("added project to bull");
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
