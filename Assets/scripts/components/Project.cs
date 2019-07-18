using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Project
{
    public IProjectable Deliverable;
    public int Phase = 1;
    public int Repetitions;
    public int CompletedCount;
    public float PercentComplete;
    public Entity Entity;
    public List<WorkReq> Requirements;
    public HashSet<KeyValuePair<string, object>> prereqs = new HashSet<KeyValuePair<string, object>>();
    public HashSet<KeyValuePair<string, object>> effects = new HashSet<KeyValuePair<string, object>>();

    public void addEffect(string s, object o) {
        effects.Add(new KeyValuePair<string, object>(s,o));
    }

    public Project(Entity entity, IProjectable deliverable, List<WorkReq> reqs, int num = 1)
    {
        Entity = entity;
        Deliverable = deliverable;
        Requirements = reqs;
    }

    public void ForceComplete() {
        complete = true;
    }

    bool complete = false;
    public bool isComplete()
    {
        if (!complete)
        {
            if (Requirements.Where(x => x.PercentComplete() < 1).Count() == 0)
            {
                Deliverable.Complete();

                CompletedCount += 1;
                if(CompletedCount >= Repetitions && Repetitions != -1) {
                    complete = true;
                    return true;
                }
                else {
                    Requirements.ForEach(x => x.Reset());
                }
            }
        }
        
        return complete;
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
        //Debug.Log(PercentComplete);
    }
}
