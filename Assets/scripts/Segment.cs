using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Segment {

    public Node startNode;
    public Node endNode;
    public float angle;
    public int intersectionCount;
    public Road road;
    public List<Lot> Lots;

    public Segment(Node _startNode, Node _endNode, Road _road) {
        startNode = _startNode;
        endNode = _endNode;
        road = _road;
        angle = Angle();
        Lots = new List<Lot>();
    }


    public void AddLot(Lot L) {
        if (!Lots.Contains(L)) Lots.Add(L);

        Dictionary<float, Lot> UnorderedLots = new Dictionary<float, Lot>();
        Lots.ForEach(x => {
            //UnorderedLots.Add(Vector3.Distance(x.RoadPoint, startNode.pos()),x);
        });
        //SortedDictionary<float, Lot> OrderedLots = new SortedDictionary<float, Lot>(UnorderedLots);
        //Lots = UnorderedLots.Values.ToList();
    }

    public float Angle() {
        Ray ray = new Ray(start(), end()-start());
        Vector3 L = Utils.ReturnMaximalVector(new List<Vector3> { start(), end() }, Utils.Left);
        Vector3 R = Utils.ReturnMaximalVector(new List<Vector3> { start(), end() }, Utils.Right);
        Vector3 vec = R - L;

        if (L.z < R.z) return -Vector3.Angle(vec, Vector3.right);
        else return Vector3.Angle(vec,Vector3.right);
    }

    public float angleV() {
        Vector3 U = Utils.ReturnMaximalVector(new List<Vector3> { start(), end() }, Utils.Up);
        Vector3 D = Utils.ReturnMaximalVector(new List<Vector3> { start(), end() }, Utils.Down);
        Vector3 vec = U - D;
        if (U.x < D.x) return Vector3.Angle(vec, Vector3.left);
        else return Vector3.Angle(vec, Vector3.left);
    }

    public void AddIntersection(Node _node) {
        /*if (!nodes.Contains(_node)) {
            nodes.Add(_node);
            intersectionCount = nodes.Count;
            //Debug.Log(road.roadName+" "+_node.transform.name);
        }*/
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
