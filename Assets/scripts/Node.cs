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

    public enum Type {
        intersection,
        turn
    }

    public Type type;

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

    void OnMouseDown() {
        Debug.Log("click");
        Selection.activeObject = this.gameObject;
    }
}
