using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[System.Serializable]
[ExecuteInEditMode]

public class Node : MonoBehaviour {


    [SerializeField]
    public List<Segment> segments = new List<Segment>();
    public List<Road> roads = new List<Road>();
    public List<Node> adjNodes = new List<Node>();

    private Node self;

    Dictionary<Node, List<Node>> PathCache;

    public enum Type {
        intersection,
        turn
    }

    public Type type;

    private void Awake()
    {
        //Cost = 2;
        self = GetComponent<Node>();
        PathCache = new Dictionary<Node, List<Node>>();
        //CachePaths();
    }

    public void Init(List<Segment> _segments = null, Type _type = Type.turn) {
        segments = _segments;
        type = _type;
    }

    public List<Road> Roads() {
        List<Road> l = new List<Road>();
        if (segments == null) Debug.Log("nullll");
        if(segments.Count > 0) segments.ForEach(x => l.Add(x.road));
        if (l.Count == 2) transform.name = l[0].roadName + " and " + l[1].roadName;
        else if(l.Count == 1)  transform.name = l[0].roadName;
        return l;
    }

    public List<Node> AdjNodes() {
        return adjNodes;
    }

    [SerializeField]
    public Vector3 pos() {
        return transform.position;
    }

    public void AddNode() {
        transform.parent.GetComponent<Road>().AddNode(GetComponent<Node>());
    }

    public void RemoveNode() {
        transform.parent.GetComponent<Road>().RemoveNode(GetComponent<Node>());
    }

    public List<Node> GetPathTo(Node node)
    {
        if (PathCache.ContainsKey(node))
        {
            return PathCache[node];
        }
        else
        {
            Debug.LogError("given node is not in path");
            return null;
        }
    }

    [InspectorButton("CachePaths")]
    public bool cache;

    public void CachePaths()
    {
        NodeMap.instance.nodes.ForEach(x =>
        {
            if (x != self)
            {
                //Debug.Log("wfoiwg");
                PathCache[x] = NodeMap.instance.ReturnPath(self, x);
            }
        });

    }

    [InspectorButton("GenArrowViz")]
    public bool arrowViz;
    public List<Node> GenArrowViz()
    {
        return NodeMap.instance.ReturnPath(self, NodeMap.instance.nodes[17],arrows:true, lazy:false);
    }

    [InspectorButton("GenNodeViz")]
    public bool nodeViz;
    public List<Node> GenNodeViz()
    {
        return NodeMap.instance.ReturnPath(self, NodeMap.instance.nodes[17],nodes:true,lazy:false);
    }


    public List<Node> CachePath(Node n)
    {
        //Debug.Log(NodeMap.instance.ReturnPath(self, n).Count);
        return NodeMap.instance.ReturnPath(self, n);
    }

    //public void CachePaths(List<Node> nodes)
    //{
    //    nodes.ForEach(x =>
    //    {
    //        if (x != self)
    //        {
    //            PathCache[x] = NodeMap.instance.ReturnPath(self, x);
    //        }
    //    });

    //}

    public float Cost;
    public float CostSoFar;

    public GameObject CameFromObj;

    void OnMouseDown() {
        Debug.Log("click");
        Selection.activeObject = this.gameObject;
    }



    public void CameFromMarker(Vector3 cameFrom)
    {
        Vector3 pos = (transform.position + cameFrom) / 2;
        GameObject arrow = (GameObject)Instantiate(Resources.Load("arrow"), pos + Vector3.up*.1f, Quaternion.identity);
        arrow.transform.LookAt(cameFrom);
    }
}
