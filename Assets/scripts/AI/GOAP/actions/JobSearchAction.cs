using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JobSearchAction : GoapAction
{
    Person person;
    Job targetJob;
    List<Job> appliedJobs = new List<Job>();

    public void Awake()
    {
        person = GetComponent<Person>();
        addEffect("hasJob", true);
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return JobBullitin.Instance.Available.Count > 0;
    }

    public override bool isDone()
    {
        if(targetJob != null && person.Job == targetJob)
        {
            person.RemoveGoal("hasJob", true);
            return true;
        }

        return false;
    }

    public override bool perform(GameObject agent)
    {
        List<Job> contenders = new List<Job>();
        List<float> ranks = new List<float>();

        foreach (var job in JobBullitin.Instance.Available)
        {
            bool valid = true;
            if (appliedJobs.Contains(job))
            {
                valid = false;
            }
            foreach (var skill in job.Skills)
            {
                bool match = false;
                foreach(var mySkill in person.Skills)
                {
                    if (mySkill >= skill)
                    {
                        match = true;
                    }
                }
                if (!match)
                {
                    valid = false;
                }
            }

            if(valid == true)
            {
                Debug.Log("valid");
                contenders.Add(job);
                ranks.Add(job.Pay); //TODO: add in distance etc
            }
        }
        if (contenders.Count > 0)
        {
            targetJob = contenders[ranks.IndexOf(ranks.Max())];

            Apply(targetJob);

            return true;
        }
        else return false;

    }

    private void Apply(Job job)
    {
        Debug.Log("apply");
        job.Manager.GetComponent<WorkerSearchAction>().AddApplicant(person,job);
        appliedJobs.Add(job);
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
        targetJob = null;
    }
}
