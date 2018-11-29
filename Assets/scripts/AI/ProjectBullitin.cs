using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ProjectBullitin
{
    public ProjectEvent AddProjectToBullitinEvent;
    public List<Project> Available = new List<Project>();

    public void Add(Project listing)
    {
        Available.Add(listing);
        AddProjectToBullitinEvent.Invoke(listing);
    }

    public void AddListener(UnityAction<Project> method)
    {
        AddProjectToBullitinEvent.AddListener(method);
    }

    public void RemoveListener(UnityAction<Project> method)
    {
        AddProjectToBullitinEvent.RemoveListener(method);
    }

    public void Remove(Project listing)
    {
        Available.Remove(listing);
    }

    private static ProjectBullitin instance;

    private ProjectBullitin() {
        AddProjectToBullitinEvent = new ProjectEvent();
    }

    public static ProjectBullitin Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ProjectBullitin();
            }

            return instance;
        }
    }
}

public class ProjectEvent : UnityEvent<Project>
{

}