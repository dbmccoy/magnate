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
    public float Width;

    public Segment(Node _startNode, Node _endNode, Road _road, float _nodeOffset = 0) {
        startNode = _startNode;
        endNode = _endNode;
        road = _road;
        angle = Angle();
        Lots = new List<Lot>();
        Width = 1;    }


    public void AddLot(Lot L) {
        if (!Lots.Contains(L)) Lots.Add(L);
    }

    public List<Lot> OrderedLots()//
    {
        Lots = Lots.OrderBy(x => x.segDistance).ToList();
        //List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();

        List<Lot> LotsL = Lots.Where(x => x.isLeftOfSegVector == true).ToList();
        List<Lot> LotsR = Lots.Where(x => x.isLeftOfSegVector == false).ToList();
        List<Lot> LotsFinal = new List<Lot>();

        bool tick = true;
        while (LotsL.Count > 0 || LotsR.Count > 0) {
            float diffL = (LotsL.Count > 0) ? LotsL[0].DistanceToSegStart() : 1000f;
            float diffR = (LotsR.Count > 0) ? LotsR[0].DistanceToSegStart() : 1000f;
            float diffLtoR = diffR - diffL;

            if (tick == true || LotsR.Count == 0) {
                if(diffLtoR > -2f || LotsR.Count == 0) {
                    LotsFinal.Add(LotsL[0]);
                    LotsL.Remove(LotsL[0]);
                }
                else {
                    LotsFinal.Add(LotsR[0]);
                    LotsR.Remove(LotsR[0]);
                }
                tick = false;
            }
            if(tick == false || LotsL.Count == 0) {
                if(diffLtoR < 2f || LotsL.Count == 0) {
                    LotsFinal.Add(LotsR[0]);
                    LotsR.Remove(LotsR[0]);
                }
                else {
                    LotsFinal.Add(LotsL[0]);
                    LotsL.Remove(LotsL[0]);
                }
                tick = true;
            }
        }
        Lots = LotsFinal;
        return LotsFinal;
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
    
    public Vector3 start() {
        return startNode.pos();
    }

    public Vector3 StartWithOffset(Vector3 lotPos, float offset = 0)
    {
        var left = start() + (Quaternion.Euler(0, 90, 0) * vector()).normalized;
        left = left + (left * offset);
        var right = start() + (Quaternion.Euler(0, -90, 0) * vector()).normalized;
        right = right + (right * offset);
        return (Vector3.Distance(lotPos,left) > Vector3.Distance(lotPos,right)) ? right : left;
    }

    public Vector3 end() {
        return endNode.pos();
    }

    public Vector3 EndWithOffset(Vector3 lotPos, float offset = 0)
    {
        var left = end() + (Quaternion.Euler(0, 90, 0) * vector()).normalized;
        left = left + (left * offset);
        var right = end() + (Quaternion.Euler(0, -90, 0) * vector()).normalized;
        right = right + (right * offset);
        return (Vector3.Distance(lotPos, left) > Vector3.Distance(lotPos, right)) ? right : left;
    }


    public Vector3 vector() {
        return start() - end();
    }
}
