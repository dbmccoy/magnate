using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections.Specialized;

public class WorkUnit {

    public Entity Entity;
    public Person Manager;
    public List<IProductive> Inputs = new List<IProductive>();
    public HashSet<Work.Type> WorkTypes = new HashSet<Work.Type>();
    public List<Project> Projects = new List<Project>();

    public HashSet<IWorkUnitAction> ActionSet = new HashSet<IWorkUnitAction>();

    public TypeDictionary<IWorkUnitAction> ActionDict = new TypeDictionary<IWorkUnitAction>();

    public void AddProject(Project project)
    {
        Projects.Add(project);
        Debug.Log("project added");
    }

    public void AddInput(IProductive input)
    {
        if(Inputs.IndexOf(input) == -1)
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

    public void CheckForNewAvailableActions(Work.Type newType)
    {
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
    }

    public void TakeInput(IProductive input)
    {
        Projects[0].OptimizeInput(input); //bootstrap
        foreach (var project in Projects)
        {
            
        }
    }
}
