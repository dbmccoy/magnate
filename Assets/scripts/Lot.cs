using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using System.Linq;

public class Lot : MonoBehaviour {

    public List<Vector3> verts;
    public List<Vector3> points;
    public Vector3 center;
    public Vector3 left;
    public Vector3 right;
    public Vector3 leftPlus;
    public Vector3 rightPlus;
    public string horizontal;
    public LotInfo info;


	// Use this for initialization
	public void init(LotInfo _info) {
	    info = _info;
	    center = info.Center;
	    verts = info.LotVerts;
	    points = info.points;
	    left = info.Left;
	    right = info.Right;
        
	    foreach (var v in verts) {
	        Instantiate(info.ParentBlock.marker, v, Quaternion.identity, transform);
        }
	    Vector3 origin = Vector3.zero;
        foreach (var v in verts) {
            origin = origin + v;
        }
	    origin = origin / verts.Count;
        Debug.Log("origin "+origin);
	    arr = Utils.V2dArray(verts);

        Array.Sort(arr,new ClockwiseComparer(origin));
	    for (int i = 0; i < arr.Length; i++) {
            AdjVerts.Add(new Vector3(arr[i].x,0,arr[i].y));
	    }
	    SetMesh(AdjVerts,arr);
    }

    public Vector3[] MeshVerts;
    public int[] triangles;
    public Vector2[] arr;
    public List<Vector3> AdjVerts;

    void SetMesh2(List<Vector3> _verts) {
        Vector2z[] v2s = new Vector2z[_verts.Count];
        for (int i = 0; i < _verts.Count; i++) {
            v2s[i] = new Vector2z(_verts[i]);
        }

        int[] indices = EarClipper.Triangulate(v2s);

        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = indices;

        mesh.RecalculateBounds();

        MeshVerts = mesh.vertices;
        triangles = indices;

        gameObject.AddComponent(typeof(MeshRenderer));

        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/lot");
        filter.mesh = mesh;
    }

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
    }

    // Update is called once per frame
    void Update () {
		
	}
}
