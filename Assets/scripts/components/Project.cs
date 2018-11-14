using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Project
{
    public IProjectable Deliverable;
    public int Phase = 1;
    public float PercentComplete;

    public List<WorkReq> Requirements;

    public Project(IProjectable deliverable, List<WorkReq> reqs)
    {
        Deliverable = deliverable;
        Requirements = reqs;
    }

    public bool isComplete()
    {
        if (Requirements.Where(x => x.PercentComplete() < 1).Count() == 0)
        {
            Debug.Log("complete");
            return true;
        }
        else return false;
    }

    public List<WorkReq> GetOpenReqs()
    {
        var reqs = Requirements.Where(x => x.Order <= Phase && x.CurrentAmount < x.MaximumAmount).ToList();
        if(reqs.Where(x => x.Order == Phase).Count() == 0)
        {
            Phase++;
            reqs = Requirements.Where(x => x.Order <= Phase && x.CurrentAmount < x.MaximumAmount).ToList();
        }
        return reqs;
    }

    public void OptimizeInput(IProductive input)
    {
        var reqs = GetOpenReqs();
        WorkReq highest = reqs.First();
        foreach (var req in reqs)
        {
            if(input.GetSkill(req.Type) >= req.Skill && input.GetSkill(req.Type) >= highest.Skill)
            {
                highest = req;
            }
        }
        highest.TakeInput(input.GetSkill(highest.Type));
        float perc = 0f;
        foreach (var req in Requirements)
        {
            perc += req.PercentComplete();
        }
        PercentComplete = perc / Requirements.Count;
        Debug.Log(PercentComplete);
    }
}
