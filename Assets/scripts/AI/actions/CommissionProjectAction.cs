using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommissionProjectAction : GoapAction, IProjectAction {

    private Person person;
    private GoapAgent Agent;

    public Entity Entity { get; set; }

    public void Awake()
    {
        person = GetComponent<Person>();
        Agent = GetComponent<GoapAgent>();
        isReusable = true;
    }

    //this is the list of all projects that the entity has generated for consideration. As projects get added to the sequence they are removed from this list.
    public List<Project> availableProjects = new List<Project>();

    //this is the list of projects that, completed in order, will satisfy the conditions for the goal
    public List<Project> projectSequence = new List<Project>();
    public List<Project> tempProjectSequence = new List<Project>();

    public Dictionary<string, List<Project>> projectMatrix = new Dictionary<string, List<Project>>();

    //the project that the entity is currently posting. The plan should iterate through each project in the project sequence and post it
    public Project currentProject;

    //the project the planner uses when checking to see if said project will help achieve the goal
    public Project tempProject { get; set; }

    public List<string> AvailableProjectNames = new List<string>();
    public List<string> ProjectSequenceNames = new List<string>();

    int checkcount = 0;

    public override bool checkProceduralPrecondition(GameObject agent) {
        checkcount++;
        Preconditions.Clear();
        Effects.Clear();
        isPosted = false;
        tempProject = null;
        //Debug.Log("tempp is null");

        //populated available projects with every project that the officer is considering, except those that have already been added to the sequence
        Entity = person.CurrentUnit.Entity;
        AvailableProjectNames.Clear();
        availableProjects = Entity.Officer.PlanningProjects.Where(x => !tempProjectSequence.Contains(x)).ToList();

        foreach (var p in availableProjects) {
            AvailableProjectNames.Add(p.Deliverable.Name + " for " + p.Entity);
        }

        if(availableProjects.Count == 0) {
            return failProceduralPreconditions();
        }

        tempProject = availableProjects.Random();

        if(tempProject == null) {
            Debug.LogError("temp project is null");
        }
        
        if (projectSequence.Contains(tempProject)) {
            throw new System.Exception("precondition check chose a tempproject that was already in the sequence");
        }

        if(tempProject.effects.Count == 0) {
            Debug.Log("no fx");
        }

        foreach (var i in tempProject.prereqs) {
            //Debug.Log(checkcount + "-" + tempProject.Deliverable.Name + " adding pre " + i.Key + ":" + i.Value.ToString());
            addPrecondition(i.Key, i.Value);
        }
        foreach (var e in tempProject.effects) {
            addEffect(e.Key, e.Value);
            //Debug.Log(checkcount + "-"+tempProject.Deliverable.Name + " adding eff " + e.Key + ":" + e.Value.ToString());
        }

        return true;
    }

    protected override bool failProceduralPreconditions() {
        
        return false;
    }
    

    protected override void hardReset() {
        
        Preconditions.Clear();
        Effects.Clear();
        tempProject = null;
        projectSequence.Clear();
        projectMatrix.Clear();

        availableProjects.Clear();
        currentProject = null;

        blackListCount = 0;
        Debug.Log("hard reset");
        
    }

    public bool reject() {
        blackListCount++;
        Debug.Log(blackListCount);
        if(blackListCount > 100) {
            hardReset();
            return true;
        }
        return false;
    }

    public override bool addToPlan(string id) {
        
        if(tempProject == null) {
            throw new System.Exception("trying to add plan w temp proj is null");
            return false;
        }

        var s = tempProject.Deliverable.Name;

        foreach (var item in tempProject.effects) {
            s += id + ": " + tempProject.Deliverable.Name + " > " + item.Key + " : " + item.Value;
        }

        //Debug.Log(s);

        if (projectMatrix.ContainsKey(id)) {
            tempProjectSequence = projectMatrix[id];
        }
        else {
            Debug.Log("new project sequence " + id);
            tempProjectSequence = new List<Project>();
            projectMatrix[id] = tempProjectSequence;
        }

        if (!tempProjectSequence.Contains(tempProject)) {
            Debug.Log("success! adding "+ tempProject.Deliverable.Name);
            var ss = "prereqs met: ";
            foreach (var item in tempProject.prereqs) {
                ss += item.Key + ":" + item.Value;
            }
            Debug.Log(ss);
            //tempProjectSequence.Insert(0,tempProject);
            tempProjectSequence.Add(tempProject);
            tempProject = null;
        }
        else {
            throw new System.Exception("alert: goap planner trying to add a tempProject that is already contained in the sequence");
        }
        //Debug.Log("projlist count = " + tempProjectSequence.Count);
        //Preconditions.Clear();
        //Effects.Clear();
        checkProceduralPrecondition(person.gameObject);
        return true;
    }

    public override void addToFinalPlan(string id) {
        projectSequence = projectMatrix[id];
        Debug.Log("final adding " + id + " : sequence length = " + projectSequence.Count);
    }

    public override bool isDone()
    {
        if(currentProject == null) {
            return false;
        }
        if(currentProject.Deliverable == null) {
            throw new System.Exception("current project deliverable is null");
        }
        var done = currentProject.isComplete();

        //Debug.Log(project.PercentComplete);

        if (done) {
            foreach (var e in currentProject.effects) {
                addEffect(e.Key,e.Value);
                Debug.Log("PROJ COMPLETE: " + currentProject.Deliverable.Name + " " + e.Key.ToString() + " : " + e.Value.ToString());
            }

            projectSequence.Remove(currentProject);
            person.PlanningProjects.Remove(currentProject);
            currentProject = null;
            isPosted = false;
            if(projectSequence.Count > 0) {
                Debug.Log("next commission project is " + projectSequence.FirstOrDefault());
            }
            else {
                Debug.Log("finished project sequence");
                hardReset();
            }
            
        }
        return done;
    }

    bool isPosted = false;

    public override bool perform(GameObject agent)
    {
        if (!isPosted)
        {
            currentProject = projectSequence.FirstOrDefault();

            if(currentProject == null) {
                throw new System.Exception("commission action is trying to post a null project");
            }

            foreach (var e in currentProject.effects) {
                addEffect(e.Key, e.Value);
                Debug.Log(currentProject.Deliverable.Name + " has effect " + e.Key.ToString() + " : " + e.Value.ToString());
            }

            isPosted = ProjectBullitin.Instance.Add(currentProject);

            if (!isPosted) {
                throw new System.Exception("commission project posting failed");
            }
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {

    }       

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
