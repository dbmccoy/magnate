using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GoapAgent))]
public class Person : MonoBehaviour, IGoap, IProductive{

    public string Name;
    public string Residence; //convert to unit

    public Job Job; //convert to Job
    public string JobName;

    public E_Race Race;
    GoapAgent agent;
    public Entity Entity;
    [HideInInspector]
    public bool isDummy;

    public Queue<Project> Projects = new Queue<Project>();
    public List<HashSet<KeyValuePair<string, object>>> GoalQueue = new List<HashSet<KeyValuePair<string, object>>>();
    public void Awake()
    {
        Name = "Jeff";
        Entity = new Entity("Entity");
        Skills = new List<Skill>();
        agent = GetComponent<GoapAgent>();
        Entity = new Entity(name);
        GameManager.Instance.People.Add(this);
        SelfUnit = Entity.WorkUnits.First();
        AssignUnit(SelfUnit);

        AddComponent<TransferAssetAction>();
        AddComponent<RentResidenceAction>();
        AddComponent<JobSearchAction>();
        AddTemporal();
    }

    public GoapAgent GetAgent()
    {
        return agent;
    }

    //IProductive
    public WorkUnit CurrentUnit { get; set; }
    public WorkUnit SelfUnit { get; private set; }
    public Project Project { get; set; }
    public List<Skill> Skills { get; set; }
    public float Capacity { get; set; }

    public void AssignUnit(WorkUnit unit)
    {
        unit.AddInput(this);
        CurrentUnit = unit;
    }

    public Skill GetSkill(SkillType type)
    {
        foreach (var skill in Skills)
        {
            if(skill.type == type)
            {
                return skill;
            }
        }
        return null;
    }

    public void AddSkill(SkillType type, float val)
    {
        if(Skills == null)
        {
            Skills = new List<Skill>();
        }
        Skills.Add(new Skill( type, val ));
    }

    public void AssignProject(Project project)
    {
        Project = project;
    }

    public void DoWork()
    {
        CurrentUnit.TakeInput(this);
        Capacity--;
    }

    public void AddTemporal()
    {
        new Temporal(this);
    }

    public List<string> CurrentGoal;
    public List<string> CurrentPlan;
    public string CurUnit;

    public void DayTick()
    {
        CurrentGoal.Clear();
        foreach (var goal in getGoals())
        {
            CurrentGoal.Add(GoapAgent.prettyPrint(goal));
        }
        CurUnit = CurrentUnit.Entity.Name;

        if (CurrentUnit != null)
        {
            DoWork();
        }
        else
        {
            //Debug.Log(Name + " no work");
        }
    }

    public void MonthTick()
    {
    }
    
    public enum E_Race
    {
        white,
        black,
        latino,
        asian
    }

    public T AddComponent<T>() where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("hasJob", (Job != null)),
            new KeyValuePair<string, object>("hasResidence", (Residence.Length > 0))
        };

        return worldData;
    }

    public void AddProject(Project p)
    {
        Project = p;
        AddGoal(p.Entity.ID+"hasAsset", Project.Deliverable);
    }

    public void AddGoal(HashSet<KeyValuePair<string, object>> goal)
    {
        GoalQueue.Add(goal);
    }

    public void AddGoal(string s, object o)
    {
        var pair = new KeyValuePair<string, object>(s, o);
        var set = new HashSet<KeyValuePair<string, object>> { pair };

        var match = false;
        foreach (var goal in GoalQueue)
        {
            if (goal.Contains(pair))
            {
                match = true;
            }
        }

        if (!match)
        {
            GoalQueue.Add(set);
        }
    }

    public void RemoveGoal(HashSet<KeyValuePair<string, object>> goal)
    {
        GoalQueue.Remove(goal);
    }

    public void RemoveGoal(string s, object o)
    {
        var goal = new KeyValuePair<string, object>(s, o);
        foreach (var item in GoalQueue) {
            if (item.Contains(goal)) {
                item.Remove(goal);
            }
            if(item.Count == 0) {
                GoalQueue.Remove(item);
                return;
            }
        }
        
        //GoalQueue.Remove(goal);
    }


    public List<HashSet<KeyValuePair<string, object>>> getGoals()
    {
        if (getWorldState().HasPair("hasResidence", false))
        {
            AddGoal("hasResidence", true);
        }
        else
        {
            RemoveGoal("hasResidence", true);
        }
        if (getWorldState().HasPair("hasJob", false))
        {
            AddGoal("hasJob", true);
        }
        else
        {
            RemoveGoal("hasJob", true);
        }

        //TODO: rank goals by priority
        return GoalQueue;
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        foreach (var item in goal)
        {
            //Debug.Log(item);
        }
        //Debug.Log(GoapAgent.prettyPrint(goal));
        //Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        //Debug.Log("<color=red>Plan Aborted</color> " + name + " " + GoapAgent.prettyPrint(aborter));
        //GoalQueue = GoalQueue.OrderBy(x => Random.value).ToList();
        foreach (var goal in GoalQueue)
        {
            //Debug.Log(name + " " + GoapAgent.prettyPrint(goal));
        }
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target

        //set this up later
        if (gameObject.transform.position.Equals(nextAction.target.transform.position)) 
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }
}


