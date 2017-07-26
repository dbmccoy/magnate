using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using System.Linq;

public class Lot : MonoBehaviour {

    public Road road;
    public Block block;
    public Segment segment;

    public List<Vector3> verts;
    public List<Vector3> buildableVerts;
    public Edge frontEdge;
    public Edge cornerEdge;
    public Dictionary<Lot,Edge> adjacentLots = new Dictionary<Lot, Edge>();
    public List<Edge> edges;
    public List<Vector3> points;
    public Vector3 center;
    public Vector3 left;
    public Vector3 right;
    public MeshCollider col;
    public string horizontal;
    public LotInfo info;
    public Vector3 direction;
    public Vector3 RoadPoint;
    public Lot self;
    public string Address;

    //segment ordering variables
    public bool isLeftOfSegVector;
    public float angleToSegStart;


	// Use this for initialization
	public void init(LotInfo _info) {
        self = GetComponent<Lot>();
	    info = _info;
	    center = info.LotCenter;
	    verts = info.LotVerts;
	    points = info.points;
	    left = info.Left;
	    right = info.Right;
        direction = info.Direction;
        block = info.ParentBlock;

	    foreach (var v in verts) {
	        //Instantiate(info.ParentBlock.marker, v, Quaternion.identity, transform);
        }
	    Vector3 origin = Vector3.zero;
        foreach (var v in verts) {
            origin = origin + v;
        }
	    origin = origin / verts.Count;
        Dictionary<float, Vector3> vertSortDict = new Dictionary<float, Vector3>();

        foreach (var v in verts) {
            if (origin.x < v.x) vertSortDict.Add(Vector3.Angle(Vector3.forward, v - origin),v);
            else vertSortDict.Add(360 - Vector3.Angle(Vector3.forward, v - origin),v);
            
        }
        SortedDictionary<float, Vector3> sortedVertSortDict = new SortedDictionary<float, Vector3>(vertSortDict);
        angles = sortedVertSortDict.Keys.ToList();
        verts = sortedVertSortDict.Values.ToList();


	    arr = Utils.V2dArray(verts);

	    SetMesh(verts,arr);
        col = gameObject.AddComponent<MeshCollider>();
        road = info.RoadSegment.road;
        segment = info.RoadSegment;
        RoadPoint = info.RoadPoint;
        road.AddLot(self);
        block.Lots.Add(self);
        Vector2 vec = Utils.V2d(segment.start() - center);
        
        isLeftOfSegVector = (Utils.AngleDir(Utils.V2d(segment.vector()), vec) < 0) ? true : false;
        angleToSegStart = Vector3.Angle(segment.vector(), segment.start() - center);
        segDistance = DistanceToSegStart();

        edges = new List<Edge>(); //CREATE LOT EDGES
        for (int i = 0; i < verts.Count-1; i++)
        {
            edges.Add(new Edge(new Vector3[]{ verts[i], verts[i + 1] }));
        }
        edges.Add(new Edge(new Vector3[] { verts[verts.Count - 1], verts[0] }));

        //Instantiate(Resources.Load("boundingPoint"), RoadPoint, Quaternion.identity, transform);
        //Instantiate(Resources.Load("boundingPoint"), segment.StartWithOffset(center),Quaternion.identity,transform  );
        //Instantiate(Resources.Load("boundingPoint"), segment.EndWithOffset(center), Quaternion.identity, transform);

        Debug.Log(RoadPoint);
        if (RoadPoint != null)
        {
            segment.AddLot(self);
            float distance = 999f;
            edges.ForEach(x =>
            {
                if(Vector3.Distance(x.mid(),RoadPoint) < distance)
                {
                    distance = Vector3.Distance(x.mid(), RoadPoint);
                    frontEdge = x;
                }
            });

            frontstart = frontEdge.start(); frontend = frontEdge.end();
            //Bug.Mark(frontEdge.start()); Bug.Mark(frontEdge.end());
        }
    }

    public Vector3 frontstart; public Vector3 frontend;

    //segment ordering vars

    public List<float> angles;

    private void OnMouseDown() {
        Debug.Log("click");
    }

    public float segDistance; //DEBUG ONLY

    public float DistanceToSegStart() {
        segDistance = Vector3.Distance(RoadPoint, segment.startNode.pos());
        return segDistance;
    }

    //public List<Vector3> ReturnBuildableVerts()
    //{
    //    float f = info.Zoning.FrontSetback;
    //    float s = info.Zoning.SideSetback;
    //    Edge frontage;
    //    edges.ForEach(x =>
    //    {
    //        info.ParentBlock.boundingSegments.ForEach(j =>
    //        {
    //            Vector3 normal = (x.start + x.vector) / 2 + (x.Normal(center) * 10f);
    //            if (Math3d.AreLineSegmentsCrossing((x.start + x.vector) / 2, normal, j.start(), j.end()))
    //            {
    //                frontage = x;
    //                return;
    //            }
    //        });
    //    });

    //    //edges.ForEach(x =>
    //    //{
    //    //    edges.ForEach(j =>
    //    //    {

    //    //        Vector3 hit;
    //    //        if (Math3d.LineLineIntersection(out hit, x.StartOffset(center,), x.vector, j.StartOffset, j.vector))
    //    //        {

    //    //        }
    //    //    });
    //    //});
        
    //}

    public void Build() {
        GameObject building = (GameObject)Instantiate(Resources.Load("buildings/house_test_1"), center, Quaternion.LookRotation(direction),transform);
        Debug.Log("call");
    }

    public Vector3[] MeshVerts;
    public int[] triangles;
    public Vector2[] arr;

    void SetMesh(List<Vector3> _verts, Vector2[] v2d) {

        Triangulator tr = new Triangulator(v2d);
        int[] indices = tr.Triangulate();
        //mesh.SetVertices(_verts);

        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = indices;
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshVerts = mesh.vertices;
        triangles = indices;

        gameObject.AddComponent(typeof(MeshRenderer));

        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/lot");
        filter.mesh = mesh;

        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = verts.Count + 1;
        Vector3 offset = new Vector3(0,.01f,0);
        List<Vector3> lrVerts = new List<Vector3>();
        verts.ForEach(i => lrVerts.Add(i + offset));
        lrVerts.Add(lrVerts[0]);
        lr.SetPositions(lrVerts.ToArray());
        lr.SetWidth(.1f, .1f);
        lr.material = (Material)Resources.Load("materials/darkgrey");
        //lr.SetColors(Color.grey, Color.grey);
    }

    

    // Update is called once per frame
    void Start () {
        frontstart = frontEdge.start(); frontend = frontEdge.end();
    }
}

[Serializable]
public class Edge
{
    public Vector3 _start;
    public Vector3 _end;
    Vector3 _vector;

    public Edge(Vector3[] pair)
    {
        _start = pair[0]; _end = pair[1];
        _vector = _start - _end;
    }

    public Vector3 Normal(Vector3 lotPos)
    {
        return (lotPos - (_start + _vector) / 2).normalized;
    }

    public Vector3 start()
    {
        return _start;
    }

    public Vector3 start(Vector3 lotPos, float offset = 0)
    {
        var left = _start + (Quaternion.Euler(0, 90, 0) * _vector).normalized * offset;
        var right = _start + (Quaternion.Euler(0, -90, 0) * _vector).normalized * offset;
        return (Vector3.Distance(lotPos, left) > Vector3.Distance(lotPos, right)) ? right : left;
    }

    public Vector3 end()
    {
        return _end;
    }

    public bool EqualTo(Edge edge)
    {
        if ((start() == edge.start() && end() == edge.end()) || start() == edge.end() && end() == edge.start()) return true;
        else return false;
    }

    public Vector3 end(Vector3 lotPos, float offset = 0)
    {
        var left = _end + (Quaternion.Euler(0, 90, 0) * _vector).normalized * offset;
        var right = _end + (Quaternion.Euler(0, -90, 0) * _vector).normalized * offset;
        return (Vector3.Distance(lotPos, left) > Vector3.Distance(lotPos, right)) ? right : left;
    }

    public Vector3 mid()
    {
        return (start() + end()) / 2;
    }
}
