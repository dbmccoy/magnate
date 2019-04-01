using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Linq;


public class bl_Floor
{
    public pb_Object pb;
    public ProBuilding bld;
    public List<Vector3> facadePoints;
    public List<pb_Face> walls;
    public pb_Face front;
    public pb_Face corner;
    public Dictionary<pb_Face, Vector3> normalsDict;
    public Dictionary<pb_Face, List<Vector3>> pointsDict;
    public Dictionary<pb_Face, List<Vector3>> centerDict;
    public Dictionary<pb_Face, bl.FaceType> faceTypeDict = new Dictionary<pb_Face, bl.FaceType>();


    public bl_Floor(pb_Object _pb, ProBuilding _bld)
    {
        pb = _pb;
        walls = GetWalls();
        pointsDict = GenFacadePoints(false);
        centerDict = GenFacadePoints(true);
        bld = _bld;
        front = GetFront();
        corner = GetCorner();
        TypeWalls();
    }

    List<pb_Face> GetWalls()
    {
        return bl.AxisFaces(pb.faces.ToList(), pb);
    }

    public void TypeWalls()
    {
        walls.ForEach(x =>
        {
            if (x == front) faceTypeDict.Add(x, bl.FaceType.front);
            else if (x == corner) faceTypeDict.Add(x, bl.FaceType.side);
            else faceTypeDict.Add(x, bl.FaceType.blank);
        });
    }

    public pb_Face GetFront()
    {
        pb_Face _front = null;
        pb.faces.ToList().ForEach(x =>
        {
            Dictionary<int, Vector3> dict = bl.IndexPos(x.edges.ToList(), pb);
            List<Vector3> verts = dict.Values.ToList();
            List<Vector3> zeroVerts = new List<Vector3>();
            verts.ForEach(j => zeroVerts.Add( new Vector3(j.x,0,j.z)));

            if (zeroVerts.Contains(bld.lot.frontEdge.start()) && zeroVerts.Contains(bld.lot.frontEdge.end()))
            {
                _front = x;
            }
        });
        //_front.material = (Material)Resources.Load("materials/buildings/green");
        if (_front == null) Debug.Log("nullll");
        return _front;
    }

    public pb_Face GetCorner()
    {
        pb_Face _corner = null;
        if (bld.lot.cornerEdge.start() == Vector3.zero && bld.lot.cornerEdge.end() == Vector3.zero) return null;
        pb.faces.ToList().ForEach(x =>
        {
            Dictionary<int, Vector3> dict = bl.IndexPos(x.edges.ToList(), pb);
            List<Vector3> verts = dict.Values.ToList();
            List<Vector3> zeroVerts = new List<Vector3>();
            verts.ForEach(j => zeroVerts.Add(new Vector3(j.x, 0, j.z)));
            if (zeroVerts.Contains(bld.lot.cornerEdge.start()) && zeroVerts.Contains(bld.lot.cornerEdge.end()))
            {
                _corner = x;
            }
        });
        //_front.material = (Material)Resources.Load("materials/buildings/green");
        return _corner;
    }

    public Quaternion WallNormal(List<Vector3> points)
    {
        Vector3 mid = Utils.AverageVectors(points); mid.y = 0;
        Vector3 vec = mid - new Vector3(points[0].x, 0, points[0].z);
        float angle = Vector3.Angle(Vector3.forward, vec);
        return Quaternion.LookRotation(vec) * Quaternion.Euler(new Vector3(0, 90, 0));
    }

    Dictionary<pb_Face, List<Vector3>> GenFacadePoints(bool center = true, bool normals = false)
    {
        Dictionary<pb_Face, List<Vector3>> dict = new Dictionary<pb_Face, List<Vector3>>();
        Dictionary<pb_Face, List<Vector3>> c_dict = new Dictionary<pb_Face, List<Vector3>>();


        int num = 0;
        walls.ForEach(x =>
        {
            List<pb_Edge> edges = bl.AxisEdges(pb, x);
            Vector3 v1 = bl.EdgeVectors(edges[0], pb)[0];
            Vector3 v2 = bl.EdgeVectors(edges[1], pb)[0];
            float length = Vector3.Distance(v1, v2);
            Vector3 vec = v2 - v1;
            int divisions = Mathf.FloorToInt(length);
            List<Vector3> points = new List<Vector3>();
            List<Vector3> centerPoints = new List<Vector3>();
            float mid = bl.FaceVerts(x, pb).Min(j => j.y) + (bl.FaceVerts(x, pb).Max(j => j.y) - bl.FaceVerts(x, pb).Min(j => j.y)) / 2;
            for (int i = 0; i <= divisions; i++)
            {
                Vector3 point = v1 + (vec * ((float)i / (float)divisions));
                point.y = mid;
                points.Add(point);
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                centerPoints.Add((points[i] + points[i + 1]) / 2);
                //else centerPoints.Add((points[i]));
            }
            dict.Add(x, points);
            c_dict.Add(x, centerPoints);
            num++;
        });
        if (center) return c_dict;
        else return dict;
    }
}