﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

//[System.Serializable]
[ExecuteInEditMode]

public class Block: MonoBehaviour {

    List<Segment> boundingSegments = new List<Segment>();
    List<Node> nodes = new List<Node>();
    public List<Road> boundingRoads;
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector3> shiftedVerts = new List<Vector3>();
    public Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer renderer;
    private GameObject marker;
    private Block self;
    
    public void InitBlock(List<Segment> _boundingSegments) {
        marker = (GameObject)Resources.Load("boundingPoint");
        self = GetComponent<Block>();
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
        filter.sharedMesh = new Mesh();
        mesh = filter.sharedMesh;
        SetMesh(verts);
        SetBackBlock();
    }

    public List<Vector3> debugPoints = new List<Vector3>();
    public Vector3 AnchorPoint;
    public float minimumFrontage;
    public Vector3 direction;

    [InspectorButton("BlockRayButton")]
    public bool blockRay;

    public void BlockRayButton() {
        bool isVertical = false;
        float length;
        int subdivisions;
        if(mesh.bounds.size.x >= mesh.bounds.size.z) {
            length = mesh.bounds.size.x;
        }
        else {
            length = mesh.bounds.size.z;
            isVertical = true;
        }
        subdivisions = Mathf.FloorToInt(length / minimumFrontage);

        Utils.UniqueList(debugPoints, BlockRay(mesh.bounds.center, Vector3.right, isVertical,reciprocal:false,baseLine:true));
        //SubdivideBlock(mesh.vertices.ToList(),mesh.bounds.center,new Vector3(0,0,1),dir_h:Utils.Direction.right,dir_v:Utils.Direction.up);
        direction = new Vector3(0,0,1);
        LotInfo lot1 = SubdivideBlock(LotFromParentBlock());
        LotInfo lot2 = SubdivideBlock(lot1);
    }



    public Vector3 RayIntersect;
    public float rayDistance;
    public Vector3 boundsSize;
    public Vector3 boundsMax;
    public Vector3 boundsMin;
    public Vector3 boundsExtents;
    public Vector3 point1;
    public Vector3 point2;


    public List<Vector3> BlockRay(Vector3 origin, Vector3 dir, bool vertical = false, bool reciprocal = true, bool baseLine = false) {
        point1 = origin;
        point2 = origin;

        var points = new List<Vector3>();
        points.Add(origin);
        Instantiate(marker, origin,Quaternion.identity,this.transform);
        Ray ray = new Ray(origin,dir*1000f);
        Ray ray2 = new Ray(origin, -dir * 1000f);
        ray2.origin = ray2.GetPoint(20f);
        ray.origin = ray.GetPoint(20f);


        foreach (var pair in shiftedPairs) {
            Vector3 intersection;
            Vector3 intersection2;

            //i'm so so so so so sorry for this
            if (Math3d.AreLineSegmentsCrossing(ray.origin, origin, pair[0], pair[1])) {
                Math3d.LineLineIntersection(out intersection, ray.origin, ray.origin - origin, pair[0], pair[0] - pair[1]);
                if (intersection.x > point1.x || (vertical == true && intersection.z > point1.z)) {
                    point1 = intersection;
                    if(baseLine) AnchorPoint = intersection;
                    points.Add(point1);
                    //Instantiate(marker, point1, Quaternion.identity, this.transform);

                }
            }
            if (reciprocal && Math3d.AreLineSegmentsCrossing(ray2.origin, origin, pair[0], pair[1])) {
                Math3d.LineLineIntersection(out intersection2, ray2.origin, ray2.origin - origin, pair[0], pair[0] - pair[1]);
                if (intersection2.x < point2.x || (vertical == true && intersection2.z < point2.z)) {
                    point2 = intersection2;
                    points.Add(point2);
                    //Instantiate(marker, point2, Quaternion.identity, this.transform);
                }
            }
        }
        
        return points;
    }




    LotInfo LotFromParentBlock() {
        LotInfo lot = new LotInfo(GetComponent<Block>(),Utils.Direction.right,Utils.Direction.up,shiftedVerts,direction,parentBlock:true);
        return lot;
    }

    LotInfo SubdivideBlock(LotInfo Lot) {
        List<Vector3> newPoints = new List<Vector3>();
        Utils.UniqueList(newPoints, debugPoints);
        Utils.UniqueList(newPoints, BlockRay(Lot.Center, direction, vertical: true, reciprocal: false));

        foreach (var vert in Lot.LotVerts) {
            if (Lot.HorizontalDirection == Utils.Direction.left && vert.x < Lot.Center.x) {
                if (Lot.VerticalDirection == Utils.Direction.up && vert.z > Lot.Center.z) {
                    newPoints.Add(vert);
                }
                if (Lot.VerticalDirection == Utils.Direction.down && vert.z < Lot.Center.z) {
                    newPoints.Add(vert);
                }
            }
            if (Lot.HorizontalDirection == Utils.Direction.right && vert.x > Lot.Center.x) {
                if (Lot.VerticalDirection == Utils.Direction.up && vert.z > Lot.Center.z) {
                    newPoints.Add(vert);
                }
                if (Lot.VerticalDirection == Utils.Direction.down && vert.z < Lot.Center.z) {
                    newPoints.Add(vert);
                }
            }
        }
        foreach (var newPoint in newPoints) {
            Instantiate(marker, newPoint, Quaternion.identity, transform);
        }
        LotInfo newLot = new LotInfo(self,Utils.Direction.right,Utils.Direction.up,newPoints,direction);
        return newLot;
    }


    void SetMesh(List<Vector3> _verts, bool offset = false) {

        Triangulator tr = new Triangulator(Utils.V2dArray(_verts));
        int[] indices = tr.Triangulate();
        mesh.SetVertices(_verts);
        mesh.triangles = indices;

        mesh.RecalculateBounds();
        if (offset) {
            GameObject indicator = Instantiate(Resources.Load("boundingPoint")) as GameObject;
            indicator.transform.position = filter.mesh.bounds.center;
            indicator.transform.parent = transform;
            var col = gameObject.AddComponent<MeshCollider>();

            boundsSize = mesh.bounds.size;
            boundsMax = mesh.bounds.max;
            boundsMin = mesh.bounds.min;
            boundsExtents = mesh.bounds.extents;
        }
    }

    public void RecalculateNormals2() {
        List<int> tris = mesh.triangles.ToList();
        Vector3[] normals = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < tris.Count; i += 3) {
            Vector3 a = mesh.vertices[tris[i]] - mesh.vertices[tris[i + 1]];
            Vector3 b = mesh.vertices[tris[i]] - mesh.vertices[tris[i + 2]];

            Vector3 normal = Vector3.Cross(a, b);
            normals[tris[i]] += normal;
            normals[tris[i + 1]] += normal;
            normals[tris[i + 2]] += normal;
        }

        List<Vector3> nList = new List<Vector3>();
        for (int i = 0; i < normals.Length; i++) {
            nList.Add(normals[i].normalized);
        }
        mesh.SetNormals(nList);
    }
    [SerializeField]
    public List<Vector3[]> shiftedPairs = new List<Vector3[]>();

    void SetBackBlock() {
        List<Vector3> newVerts = new List<Vector3>();
        Vector3 center = mesh.bounds.center;
        foreach (var vert in verts) {
            Vector3 v = Vector3.ClampMagnitude((vert - center), (vert - center).magnitude * .8f);
            newVerts.Add(center + v);
        }

        foreach (var segment in boundingSegments) {
            Vector3[] pair = { segment.start(), segment.end() };
            for (int i = 0; i < 2; i++) {
                pair[i] = center + Vector3.ClampMagnitude((pair[i] - center), (pair[i] - center).magnitude * .8f);

            }
            shiftedPairs.Add(pair);
        }
        shiftedVerts = newVerts;
        SetMesh(newVerts, true);
    }

}

public class LotInfo {
    public Block ParentBlock;
    public Utils.Direction HorizontalDirection;
    public Utils.Direction VerticalDirection;
    public List<Vector3> LotVerts;
    public Vector3 AnchorPoint;
    public Vector3 Center;
    public Vector3 Direction;
    
    public LotInfo(Block block,Utils.Direction h_dir,Utils.Direction v_dir,List<Vector3> lotVerts, Vector3 direction, bool parentBlock = false) {
        ParentBlock = block;
        HorizontalDirection = h_dir;
        VerticalDirection = v_dir;
        LotVerts = lotVerts;
        Direction = direction;
        AnchorPoint = block.AnchorPoint;
        if (!parentBlock) {
            Center = (block.mesh.bounds.center + AnchorPoint) / 2;
        }
        else {
            Center = block.mesh.bounds.center;
        }
    }
}
