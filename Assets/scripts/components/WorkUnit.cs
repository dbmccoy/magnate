using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.Specialized;

public class WorkUnit {

    public Entity Entity;
    public Person Manager;
    public List<IProductive> Inputs = new List<IProductive>();
    public HashSet<Work> WorkTypes = new HashSet<Work>();
    public List<Project> Projects = new List<Project>();

    public HashSet<IWorkUnitAction> ActionSet = new HashSet<IWorkUnitAction>();

    public TypeDictionary<IWorkUnitAction> ActionDict = new TypeDictionary<IWorkUnitAction>();

    public void AddProject(Project project)
    {
        if (!Projects.Contains(project))
        {
            Projects.Add(project);
            Debug.Log(project.Deliverable.ToString() + " project added");
        }
    }

    public void AddInput(IProductive input)
    {
        if (Inputs.IndexOf(input) == -1)
        {
            Inputs.Add(input);

            foreach (var i in input.Skills.Keys)
            {
                if (!WorkTypes.Contains(i))
                {
                    WorkTypes.Add(i);
                    CheckForNewAvailableActions(i);
                }
            }
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
                p.GetAgent().CurrentAction().Exit();
            }
        }
    }



    public void CheckForNewAvailableActions(Work newType)
    {
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
        }*/
    }


}
