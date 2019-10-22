using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(GoapAgent))]
public class Person : MonoBehaviour, IGoap, IProductive{

    public string Name;
    [SerializeField]
    public Unit Residence;

    public Job Job;
    public string JobName;

    public E_Race Race;
    GoapAgent agent;
    public Entity Entity;
    public Entity CurrentEntity;
    public Personality Personality;
    [HideInInspector]
    public bool isDummy;

    public bool DeepDebug;

    public Queue<Project> Projects = new Queue<Project>();
    public List<Project> PlanningProjects = new List<Project>();
    public List<HashSet<KeyValuePair<string, object>>> GoalQueue = new List<HashSet<KeyValuePair<string, object>>>();
    public void Awake()
    {
        Entity = new Entity(name);
        Entity.Officer = this;
        CurrentEntity = Entity;
        Skills = new List<Skill>();
        agent = GetComponent<GoapAgent>();

        GameManager.Instance.People.Add(this);

        SelfUnit = Entity.WorkUnits.First();
        AssignUnit(SelfUnit);

        AddComponent<TransferAssetAction>();
        AddComponent<SellAssetAction>();
        AddComponent<BuyAssetAction>();
        AddComponent<AcquireResidenceAction>();
        AddComponent<JobSearchAction>();
        AddTemporal();
    }

    //Sensors

    List<Sensor> sensors = new List<Sensor>();

    public void AddSensor(Sensor s) {
        sensors.Add(s);
    }

    public void RemoveSensor(Sensor s) {
        sensors.Remove(s);
        //update permissions matrix?
    }

    public List<Sensor> GetSensors() {
        return sensors;
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
        GetAgent().planner.ToggleDeepDebug(DeepDebug);

        if(CurrentGoal != null) {
            CurrentGoal.Clear();

            foreach (var goal in getGoals()) {
                CurrentGoal.Add(GoapAgent.prettyPrint(goal));
            }
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

    HashSet<KeyValuePair<string, object>> worldState = new HashSet<KeyValuePair<string, object>>();



    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("hasJob", (Job != null)),
            //new KeyValuePair<string, object>("hasResidence", (Residence != null))
        };

        sensors.ForEach(x => worldData.UnionWith(x.ReturnWorldData()));

        return worldData;
    }

    public void AddProject(Project p)
    {
        Debug.Log(Name + " Adding Project " + p.Deliverable.Name);
        Project = p;
        AddGoal(p + "complete", true);
        //AddGoal(p.Entity.ID+"hasAsset", Project.Deliverable.Name);
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

        if(GoalQueue.Count == 0) {
            currentPlan = "<color=green>idle</color>";
        }

        //TODO: rank goals by priority
        return GoalQueue;
    }

    public HashSet<KeyValuePair<string,object>> FindGoals(string s) {
        var matches = new HashSet<KeyValuePair<string, object>>();

        foreach(HashSet<KeyValuePair<string, object>> set in getGoals()) {
            foreach(var g in set) {
                if (g.Key.Contains(s)) {
                    matches.Add(g);
                }
            }
        }

        return matches;
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        currentPlan = "<color=red>no plan</color>";
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    private string currentPlan;
    public string GetPlan() {
        return currentPlan;
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        foreach (var item in goal)
        {
            //Debug.Log(item);
        }
        //Debug.Log(GoapAgent.prettyPrint(goal));
        // Debug.Log("<color=green>" + Name + ": Plan found</color> " + GoapAgent.prettyPrint(actions) + ": " + GoapAgent.prettyPrint(goal));
        currentPlan = "";// GoapAgent.prettyPrint(actions);

        var cpcount = 0;

        foreach (var action in actions) {

            string col = "<color=blue>";

            if(action == GetAgent().CurrentAction()) {
                col = "<color=green>";
            }
            if(action is CommissionProjectAction cp) {
                try {
                    currentPlan += col + " commission " + cp.projectSequence[cpcount].Deliverable.Name + "</color>" + "\n";
                    cpcount++;
                }
                catch {

                }
            }
            else {
                currentPlan += col + GoapAgent.prettyPrint(action) + "</color>" + "\n";
            }
            

        }
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this goal. Hooray!
        Debug.Log("<color=blue>" + Name + " Actions completed</color>");
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


