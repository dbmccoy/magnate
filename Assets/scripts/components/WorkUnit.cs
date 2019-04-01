using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Specialized;

public class WorkUnit {

    public Entity Entity;
    public Person Manager;
    public List<IProductive> Inputs = new List<IProductive>();
    public List<Skill> Skills = new List<Skill>();
    public List<Project> Projects = new List<Project>();
    public HashSet<GoapAction> ActionSet = new HashSet<GoapAction>();

    public TypeDictionary<IWorkUnitAction> ActionDict = new TypeDictionary<IWorkUnitAction>();

    public WorkUnit(Entity entity)
    {
        Entity = entity;
    }

    public void AddProject(Project project)
    {
        if (!Projects.Contains(project))
        {
            Projects.Add(project);
            //Debug.Log(project.Deliverable.Name.ToString() + " project added");
        }
    }

    public void AddInput(IProductive input)
    {
        Inputs.Add(input);

        foreach (var skill in input.Skills)
        {
            if (!Skills.Contains(skill))
            {
                Skills.Add(skill);
                CheckForNewAvailableActions(skill);
            }
        }
        if (Inputs.IndexOf(input) == -1)
        {
            
        }
    }

    public void TakeInput(IProductive input)
    {
        if(Projects.Count > 0)
        {
            if (input.Skills.Count > 0)
            {
                Projects[0].OptimizeInput(input); //bootstrap
            }
            if (Projects[0].isComplete())
            {
                CompleteProject(Projects[0]);
            }
        }
    }

    public void CompleteProject(Project project)
    {
        Projects.Remove(project);
        foreach(var input in Inputs)
        {
            if (input is Person p)
            {
                //do completion here?
            }
        }
    }



    public void CheckForNewAvailableActions(Skill skill)
    {
        var allActions = ActionManager.Instance.Actions;

        //foreach( var action in allActions.Where(x => x.SkillReqs))


        //foreach (var action in allActions.Where(x => ActionSet.Contains((GoapAction)x)))// && x.SkillReqs.Contains(skill.Key)))
        //{
            //ActionSet.Add(action as GoapAction);
        //}

        //rewrite this for monobehaviors
        /*
        foreach (var action in WorkUnitActions.Instance.Actions.Where(x => x.Value.Contains(newType)))
        {
            if (action.Value.IsSubsetOf(WorkTypes))
            {
                IWorkUnitAction newAction = (IWorkUnitAction)(System.Activator.CreateInstance(action.Key));
                newAction.Init(this);
                ActionDict.Add(action.Key, newAction);
                Debug.Log("added a new action idk");
            }
        }
        */
    }


}
