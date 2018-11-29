using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorkerSearchAction : GoapAction, IProjectAction
{
    private Person person;
    private GoapAgent agent;

    public Entity Entity { get; set; }
    public WorkUnit WorkUnit { get; set; }

    public Project Project { get; set; }
    private List<Job> OpenJobs = new List<Job>();
    private List<Person> Applicants = new List<Person>();
    private Dictionary<Job, List<Person>> JobApplications = new Dictionary<Job, List<Person>>();

    public void Awake()
    {
        person = GetComponent<Person>();
        WorkUnit = person.CurrentUnit; //TODO: fix
        agent = GetComponent<GoapAgent>();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Project = person.Project;
        addEffect("meetsWorkReqs", Project);
        return true; //WorkUnit.Manager == person;
    }

    public override bool isDone()
    {
        var missingSkills = new List<Skill>();
        foreach (var workReq in Project.Requirements)
        {
            bool match = false;
            foreach (var skill in WorkUnit.Skills)
            {
                if (skill >= workReq.Skill)
                {
                    match = true;
                }
            }

            if (!match)
            {
                missingSkills.Add(workReq.Skill);
            }
        }
        return missingSkills.Count == 0;
    }

    public bool inProgress = false;
    private List<Skill> postedSkills = new List<Skill>(); //TODO: this is temp to prevent endlessly posting the same job

    public override bool perform(GameObject agent)
    {
        var missingSkills = new List<Skill>();
        foreach (var workReq in Project.Requirements)
        {
            bool match = false;
            foreach (var skill in WorkUnit.Skills)
            {
                if (skill >= workReq.Skill)
                {
                    match = true;
                }
            }

            if (!match)
            {
                missingSkills.Add(workReq.Skill);
            }
        }


        foreach (var skill in missingSkills)
        {
            if (!postedSkills.Contains(skill))
            {
                Job newPosting = new Job(Entity, WorkUnit, new List<Skill> { skill }, person);
                //calculate how much to pay
                newPosting.Pay = 1000;

                JobBullitin.Instance.Add(newPosting);
                OpenJobs.Add(newPosting);
                postedSkills.Add(skill);
                Debug.Log("now there's a job posted");
            }
        }

        foreach (var job in OpenJobs)
        {
            Person topApplicant = null;

            if (JobApplications.ContainsKey(job))
            {
                List<Person> applicants = JobApplications[job];
                List<float> ranks = new List<float>();

                foreach (var applicant in applicants)
                {
                    //some fitness function
                    ranks.Add(Random.Range(0, 10));
                }

                topApplicant = applicants[ranks.IndexOf(ranks.Max())];
                //if fitness is above some threshold
                Hire(topApplicant, job);
            }
        }

        return true;
    }

    public void AddApplicant(Person p, Job job)
    {
        List<Person> apps = new List<Person>();
        if (JobApplications.ContainsKey(job))
        {
            apps = JobApplications[job];
        }
        apps.Add(p);
        JobApplications[job] = apps;
    }

    public void Hire(Person p, Job job)
    {
        Debug.Log("hire");
        p.JobName = "sample job";
        p.Job = job;
        JobBullitin.Instance.Remove(job);
        p.AssignUnit(WorkUnit);
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
        removeEffect("meetsWorkReqs");
    }   
}
