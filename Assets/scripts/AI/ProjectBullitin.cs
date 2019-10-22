using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ProjectBullitin
{

    public ProjectEvent AddProjectToBullitinEvent;
    public List<Project> Available = new List<Project>();

    public bool Add(Project listing)
    {
        Debug.Log("add " + listing + " " + listing.Deliverable.Name);
        Available.Add(listing);
        AddProjectToBullitinEvent.Invoke(listing);

        if (Available.Contains(listing)) {
            return true;
        }
        else return false;
    }

    public void Remove(Project listing) {
        Available.Remove(listing);
    }

    public void AddListener(UnityAction<Project> method) {
        AddProjectToBullitinEvent.AddListener(method);
    }

    public void RemoveListener(UnityAction<Project> method) {
        AddProjectToBullitinEvent.RemoveListener(method);
    }

    private ProjectBullitin() {
        AddProjectToBullitinEvent = new ProjectEvent();
    }

    private static ProjectBullitin instance;

    public static ProjectBullitin Instance {
        get {
            if (instance == null) {
                instance = new ProjectBullitin();
            }

            return instance;
        }
    }

}

public class ProjectEvent : UnityEvent<Project>
{

}