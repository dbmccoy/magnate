using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Segment {
    
    public Node startNode;
    public Node endNode;
    public List<Node> nodes = new List<Node>();
    public int intersectionCount;
    public Road road;

    public Segment(Node _startNode, Node _endNode) {
        startNode = _startNode;
        endNode = _endNode;
    }

    public Segment(Node _startNode, Node _endNode, Road _road) {
        startNode = _startNode;
        endNode = _endNode;
        road = _road;
    }

    public void AddIntersection(Node _node) {
        if (!nodes.Contains(_node)) {
            nodes.Add(_node);
            intersectionCount = nodes.Count;
            //Debug.Log(road.roadName+" "+_node.transform.name);
        }
    }

    public Vector3 start() {
        return startNode.pos();
    }

    public Vector3 end() {
        return endNode.pos();
    }

    public Vector3 vector() {
        return start() - end();
    }

    /*public Segment(Vector3 _start, Vector3 _end, Road _road) {
        start = _start;
        end = _end;
        vector = _start - _end;
        road = _road;
    }*/

    /*public Segment(Vector3 _start, Vector3 _end) {
        start = _start;
        end = _end;
        vector = _start - _end;
    }*/

}
