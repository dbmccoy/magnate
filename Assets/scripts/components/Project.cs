using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Project
{
    public IProjectable Deliverable;
    public int Phase = 1;

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
        Debug.Log("optimize input");
        var reqs = GetOpenReqs();
        WorkReq highest = reqs.First();
        foreach (var req in reqs)
        {
            if(input.Skills[req.Type] >= req.Quality && req.Quality > highest.Quality)
            {
                highest = req;
            }
        }
        highest.TakeInput(input.Skills[highest.Type]);
    }
}
