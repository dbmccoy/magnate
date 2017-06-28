using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using System.Linq;

public class Lot : MonoBehaviour {

    public Road road;
    public Segment segment;

    public List<Vector3> verts;
    public List<Vector3> points;
    public Vector3 center;
    public Vector3 left;
    public Vector3 right;
    public Vector3 leftPlus;
    public Vector3 rightPlus;
    public MeshCollider col;
    public string horizontal;
    public LotInfo info;
    public Vector3 direction;
    public Vector3 RoadPoint;
    public Lot self;
    public string Address;


	// Use this for initialization
	public void init(LotInfo _info) {
        self = GetComponent<Lot>();
	    info = _info;
	    center = info.Center;
	    verts = info.LotVerts;
	    points = info.points;
	    left = info.Left;
	    right = info.Right;
        direction = info.Direction;

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
        AdjVerts = sortedVertSortDict.Values.ToList();


	    arr = Utils.V2dArray(AdjVerts);


     //   Array.Sort(arr,new ClockwiseComparer(origin));
	    //for (int i = 0; i < arr.Length; i++) {
     //       AdjVerts.Add(new Vector3(arr[i].x,0,arr[i].y));
	    //}
	    SetMesh(AdjVerts,arr);
        col = gameObject.AddComponent<MeshCollider>();
        road = info.RoadSegment.road;
        segment = info.RoadSegment;
        RoadPoint = info.RoadPoint;

        if(RoadPoint != null)segment.AddLot(self);

    }

    public List<float> angles;

    private void OnMouseDown() {
        Debug.Log("click");
    }

    public void Build() {
        GameObject building = (GameObject)Instantiate(Resources.Load("building"), GetComponent<MeshFilter>().mesh.bounds.center, Quaternion.identity);
        building.transform.SetParent(transform);
    }

    public Vector3[] MeshVerts;
    public int[] triangles;
    public Vector2[] arr;
    public List<Vector3> AdjVerts;
    


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
        lr.numPositions = AdjVerts.Count + 1;
        Vector3 offset = new Vector3(0,.01f,0);
        List<Vector3> lrVerts = new List<Vector3>();
        AdjVerts.ForEach(i => lrVerts.Add(i + offset));
        lrVerts.Add(lrVerts[0]);
        lr.SetPositions(lrVerts.ToArray());
        lr.SetWidth(.1f, .1f);
        lr.material = (Material)Resources.Load("materials/darkgrey");
        //lr.SetColors(Color.grey, Color.grey);
    }

    

    // Update is called once per frame
    void Update () {
		
	}
}
