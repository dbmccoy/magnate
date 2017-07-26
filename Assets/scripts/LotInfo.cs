using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotInfo
{
    public Block ParentBlock;
    public LotZoningData Zoning;
    public Utils.Direction HorizontalDirection;
    public Utils.Direction VerticalDirection;
    public List<Vector3> LotVerts;
    public List<Vector3> points;
    public Vector3 Center;
    public Vector3 Left;
    public Vector3 Right;
    public Vector3 Direction;
    public Vector3 LotCenter;
    public LotInfo ParentLot;
    public float Frontage;
    public Segment RoadSegment;
    public List<Vector3> RoadFacingVerts;
    public Vector3 RoadPoint;

    public LotInfo(Block block, List<Vector3> lotVerts, Vector3 direction, Vector3 left, Vector3 right, bool parentBlock = false, LotInfo parentLot = null)
    {
        ParentBlock = block;
        ParentLot = parentLot;
        LotVerts = lotVerts;
        Direction = direction;
        Left = left;
        Right = right;
        Center = (Left + Right) / 2;
        Frontage = (right.x - left.x);
        Zoning = new LotZoningData();

        List<Segment> segments = block.boundingSegments;
        RoadFacingVerts = new List<Vector3>();
        LotCenter = Utils.AverageVectors(lotVerts);
        Vector3 _dir = new Vector3(0, 0, (LotCenter - ParentBlock.BlockCenter).z).normalized * 10f;
        Direction = _dir;
        if (_dir == Vector3.zero)
        {
            //Debug.Log("center " + LotCenter + " parent " + ParentBlock.BlockCenter);
        }

        for (int j = 0; j < segments.Count; j++)
        {
            if (Math3d.AreLineSegmentsCrossing(LotCenter, LotCenter + _dir, segments[j].start(), segments[j].end()))
            {
                RoadSegment = segments[j];
                if (Math3d.LineLineIntersection(out RoadPoint,
                    LotCenter, _dir, segments[j].start(), segments[j].vector())) return;
            }
        }

        for (int i = 0; i < lotVerts.Count; i++)
        {
            if (RoadSegment == null) return;
            Vector3 onLine = Math3d.ProjectPointOnLine(RoadSegment.StartWithOffset(LotCenter,1), RoadSegment.vector(), LotVerts[i]);
            if(Vector3.Distance(onLine,LotVerts[i]) < .1f)
            {
                RoadFacingVerts.Add(LotVerts[i]);
            }
        }
        //Debug.Log(RoadFacingVerts.Count);
        //Frontage = Vector3.Distance(RoadFacingVerts[0],RoadFacingVerts[1]);
    }
}
