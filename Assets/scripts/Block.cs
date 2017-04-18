using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

//[System.Serializable]
public class Block: MonoBehaviour {

    List<Segment> boundingSegments = new List<Segment>();
    List<Node> nodes = new List<Node>();
    public List<Road> boundingRoads;
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector3> shiftedVerts = new List<Vector3>();
    Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer renderer;
    private GameObject marker;
    
    public void InitBlock(List<Segment> _boundingSegments) {
        marker = (GameObject)Resources.Load("boundingPoint");
        foreach (var item in _boundingSegments) {
            if (!boundingSegments.Contains(item)) {
                boundingSegments.Add(item);
                //item.nodes.ForEach(i => Debug.Log(i.transform.name));              
            }
        }
        //Debug.Log(boundingSegments.Count);
        //boundingSegments.Where(i => !verts.Contains(i.start())).ToList().ForEach(j => verts.Add(j.nodes[0].transform.position));

        foreach (var seg in boundingSegments) {
            if(!nodes.Contains(seg.startNode)) nodes.Add(seg.startNode);
            if(!nodes.Contains(seg.endNode)) nodes.Add(seg.endNode);
        }
        foreach (var item in nodes) {
            verts.Add(item.transform.position);
        }

        //boundingSegments.ForEach(i => i.nodes.Where(j => !verts.Contains(j.transform.position)).ToList().ForEach(k => verts.Add(k.transform.position)));
        //nodes.Where(i => !verts.Contains(i.transform.position)).ToList().ForEach(j => verts.Add(j.transform.position));

        boundingSegments.ForEach(i => boundingRoads.Add(i.road));
        //nodes.ForEach(i => Instantiate(Resources.Load("boundingPoint"), i.pos(), Quaternion.identity));  

        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        renderer.material = (Material)Resources.Load("materials/lot");
        filter.mesh = new Mesh();
        mesh = filter.mesh;
        SetMesh(verts);
        SetBackBlock();
    }


    [InspectorButton("BlockRay")]
    public bool blockRay;

    public Vector3 RayIntersect;
    public float rayDistance;

    public void BlockRay() {
        Ray ray = new Ray(mesh.bounds.center,Vector3.right*10f);
        float distance;
        mesh.bounds.IntersectRay(ray, out distance);
        rayDistance = Mathf.Abs(distance);
        //Debug.DrawRay(mesh.bounds.center, Vector3.right, Color.blue);
        marker = (GameObject) Resources.Load("boundingPoint");
        GameObject m = Instantiate(marker,transform);
        Vector3 right = mesh.bounds.center + new Vector3(rayDistance, 0, 0);
        GameObject m2 = Instantiate(marker, transform);
        Ray ray2 = new Ray(mesh.bounds.center, Vector3.left * 10f);
        mesh.bounds.IntersectRay(ray2, out distance);
        rayDistance = Mathf.Abs(distance);
        Vector3 left = mesh.bounds.center + new Vector3(-rayDistance, 0, 0);
        Debug.DrawLine(left,right,Color.blue);
    }

    void SetMesh(List<Vector3> _verts) {

        Triangulator tr = new Triangulator(Utils.V2dArray(_verts));
        int[] indices = tr.Triangulate();
        filter.mesh.SetVertices(_verts);
        filter.mesh.triangles = indices;

        filter.mesh.RecalculateNormals();
        filter.mesh.RecalculateBounds();
        GameObject indicator = Instantiate(Resources.Load("boundingPoint")) as GameObject;
        indicator.transform.position = filter.mesh.bounds.center;
        indicator.transform.parent = transform;
    }

    void SetBackBlock() {
        List<Vector3> newVerts = new List<Vector3>();
        Vector3 center = mesh.bounds.center;
        foreach (var vert in verts) {
            Vector3 v = Vector3.ClampMagnitude((vert - center),(vert - center).magnitude * .8f);
            Debug.Log(mesh.bounds.extents.magnitude);
            Debug.Log(mesh.bounds.extents.magnitude - 2f);
            newVerts.Add(center + v);
        }
        shiftedVerts = newVerts;
        SetMesh(newVerts);
    }
}
