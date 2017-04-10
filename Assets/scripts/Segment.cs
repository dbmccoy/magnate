using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment {
    
    public Node startNode;
    public Node endNode;
    public List<Intersection> intersections = new List<Intersection>();
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

    public void AddIntersection(Intersection _intersection) {
        if (!intersections.Contains(_intersection)) {
            intersections.Add(_intersection);
            Debug.Log(road.roadName+" "+_intersection.transform.name);
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
