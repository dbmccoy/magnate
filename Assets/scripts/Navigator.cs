using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]

public class Navigator : MonoBehaviour {

    public Node CurrentNode;
    public Node GoalNode;
    public Node NextNode;
    public List<Node> Path;
    public float speed;
    public bool turns;
    public bool trail;

    private NodeMap map;
    private LineRenderer lr;
    private List<Vector3> waypoints;


	// Use this for initialization
	void Start () {
        lr = gameObject.AddComponent<LineRenderer>();
        map = NodeMap.instance;
        transform.position = CurrentNode.pos();
        StartCoroutine(EndOfFrame());
        lr.enabled = false;
        lr.SetWidth(.75f, .75f);
	}

    IEnumerator EndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        SetGoal(GoalNode);
    }
	
	// Update is called once per frame
	void Update () {
        if (NextNode)
        {
            if(Vector3.Distance(transform.position,NextNode.pos()) < .2f)
            {
                SetNextNode(Path.FirstOrDefault());
            }
            transform.Translate((NextNode.pos()-transform.position).normalized*Time.deltaTime*speed);
            waypoints[waypoints.Count - 1] = transform.position + Vector3.up * .2f;
            lr.positionCount = waypoints.Count;
            lr.SetPositions(waypoints.ToArray());
        }
	}

    public void SetGoal(Node goal)
    {
        GoalNode = goal;
        Path = CurrentNode.GetPathTo(goal);
        Debug.Log(Path.Count);
        waypoints = new List<Vector3>();
        waypoints.Add(transform.position + Vector3.up*.2f);
        waypoints.Add(transform.position + Vector3.up * .2f);
        lr.positionCount = waypoints.Count();
        lr.SetPositions(waypoints.ToArray());
        lr.enabled = true;
        SetNextNode(Path.FirstOrDefault());
    }

    public void SetNextNode(Node next)
    {
        Debug.Log(waypoints.Count);
        if(NextNode) waypoints.Insert(waypoints.Count - 1 , NextNode.pos() + Vector3.up * .2f);
        NextNode = next;
        if(turns)GetComponentInChildren<MeshFilter>().transform.LookAt(NextNode.pos());
        Path.Remove(next);
    }
}
