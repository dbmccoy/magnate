using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[ExecuteInEditMode]

public class Node : MonoBehaviour {

    [SerializeField]
    public List<Segment> segments = new List<Segment>();
    public enum Type {
        intersection,
        turn
    }

    public Type type;

    public void Init(List<Segment> _segments, Type _type = Type.turn) {
        segments = _segments;
        type = _type;
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
