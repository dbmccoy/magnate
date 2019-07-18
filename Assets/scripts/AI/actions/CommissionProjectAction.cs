using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        isReusable = true;
    }

    public int loop = 0;

    private List<Project> queuedProjects = new List<Project>();
    private List<Project> plannedProjects = new List<Project>();

    public Project tempProject { get; set; }

    public override bool checkProceduralPrecondition(GameObject agent) {
        Preconditions.Clear();
        Effects.Clear();

        if (loop >= queuedProjects.Count) {
            return false;
        }

        //Debug.Log("cp " + loop + " " + queuedProjects.Count);

        Entity = person.CurrentUnit.Entity;

        tempProject = queuedProjects.Random();//[loop];//.Where(x => !blacklist.Contains(x)).FirstOrDefault();
            //person.PlanningProjects.Where(x => !queuedProjects.Contains(x)).ToList().Random();
    
        if(tempProject != null) {
            //Debug.Log("it me");
            foreach (var i in tempProject.prereqs) {
                //Debug.Log("adding pre " + i.Key + ":" + i.Value.ToString());
                addPrecondition(i.Key, i.Value);
            }
            foreach (var e in tempProject.effects) {
                addEffect(e.Key, e.Value);
                //Debug.Log(tempProject.Deliverable.Name + " adding eff " + e.Key + ":" + e.Value.ToString());
            }
            return true;
        }
        return false;
    }

    public void NextProject() {
        loop++;

        if (loop >= queuedProjects.Count) {
            //loop = 0;
        }
    }

    public override void addToPlan() {
        Preconditions.Clear();
        Effects.Clear();

        if (!plannedProjects.Contains(tempProject)) {
            plannedProjects.Add(tempProject);
        }

        checkProceduralPrecondition(person.gameObject);
    }

    Project project;

    public override bool isDone()
    {
        if(project == null || project.Deliverable == null) {
            return false;
        }
        var done = project.isComplete();
        if (done) {
            foreach (var e in project.effects) {
                addEffect(e.Key,e.Value);
                Debug.Log("PROJ COMPLETE: " + project.Deliverable.Name + " " + e.Key.ToString() + " : " + e.Value.ToString());
            }

            //person.RemoveGoal(Entity.ID + "hasAsset", project.Deliverable.Name);
            person.PlanningProjects.Remove(project);
            queuedProjects.Remove(project);
            plannedProjects.Remove(project);
            tempProject = null;
            project = null;

            Preconditions.Clear();
            Effects.Clear();

            isPosted = false;
        }
        return done;
    }

    public int ProjQueueLength() {
        return queuedProjects.Count;
    }

    public void ClearProjectQueue() {
        queuedProjects.Clear();
    }

    public void EnqueueProject(Project p) {
        queuedProjects.Add(p);
    }

    bool isPosted = false;

    public override bool perform(GameObject agent)
    {
        if (!isPosted)
        {
            project = plannedProjects.FirstOrDefault();

            for (int i = 0; i < plannedProjects.Count; i++) {
                Debug.Log(i + " " + plannedProjects[i].Deliverable.Name);
            }

            if (project != null) {
                isPosted = true;

                foreach (var e in project.effects) {
                    addEffect(e.Key, e.Value);
                    Debug.Log(project.Deliverable.Name + " " + e.Key.ToString() + " : " + e.Value.ToString());
                }

                ProjectBullitin.Instance.Add(project);
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

        loop = 0;
        //Preconditions.Clear();
        //Effects.Clear();
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
