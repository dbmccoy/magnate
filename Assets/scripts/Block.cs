using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

//[System.Serializable]
[ExecuteInEditMode]

public class Block: MonoBehaviour {

    public List<Segment> boundingSegments = new List<Segment>();
    List<Node> nodes = new List<Node>();
    public List<Road> boundingRoads;
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector3> shiftedVerts = new List<Vector3>();
    public Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer renderer;
    private GameObject marker;
    public GameObject redMarker;
    private Block self;
    
    public void InitBlock(List<Segment> _boundingSegments) {
        marker = (GameObject)Resources.Load("boundingPoint");
        redMarker = (GameObject)Resources.Load("redMarker");
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
    public float minimumFrontage;
    public Vector3 direction;
    public Vector3 LeftAnchor;
    public Vector3 RightAnchor;

    public float X_angle;
    public Vector3 Transformed_Dir;
    [InspectorButton("AngleCheck")]
    public bool angleCheck;

    public void AngleCheck() {
        Ray ray = new Ray(self.mesh.bounds.center, Vector3.back * 1000f);
        Debug.DrawLine(ray.origin,ray.GetPoint(10));
        float X_angle_bot = 0;
        float X_angle_top = 0;
        foreach (var segment in boundingSegments) {
            if (Math3d.AreLineSegmentsCrossing(ray.origin, ray.GetPoint(20f), segment.start(), segment.end())) {
                X_angle_bot = Vector3.Angle(segment.end() - segment.start(), Vector3.back) - 90f;
                Debug.Log(X_angle_bot + "bot");
                //Debug.DrawLine(segment.start(),segment.end(),Color.blue);
            }
            if (Math3d.AreLineSegmentsCrossing(ray.origin, -ray.GetPoint(20f), segment.start(), segment.end())) {
                X_angle_top = Vector3.Angle(segment.end() - segment.start(), Vector3.back) - 90f;
                Debug.Log(X_angle_top + "top");
                //Debug.DrawLine(segment.start(),segment.end(),Color.blue);
            }
        }
        Transformed_Dir = Quaternion.Euler(0,(-X_angle_bot - X_angle_top)/2,0)*Vector3.right;
    }

    [InspectorButton("BlockRayButton")]
    public bool blockRay;

    public void BlockRayButton() {
        bool isVertical = false;
        float length;
        if(mesh.bounds.size.x >= mesh.bounds.size.z) {
            length = mesh.bounds.size.x;
        }
        else {
            length = mesh.bounds.size.z;
            isVertical = true;
        }
        AngleCheck();

        RightAnchor = Utils.ReturnMaximalVector(BlockRay(mesh.bounds.center, Transformed_Dir, isVertical, baseLine: true),Utils.Right);
        LeftAnchor = Utils.ReturnMaximalVector(BlockRay(mesh.bounds.center, -Transformed_Dir, isVertical, baseLine: true),Utils.Left);

        Utils.VectorList(shiftedVerts, RightAnchor);
        Utils.VectorList(shiftedVerts, LeftAnchor);
        Utils.VectorList(shiftedVerts, BlockRay(mesh.bounds.center, Vector3.up, true, baseLine: false));
        Utils.VectorList(shiftedVerts, BlockRay(mesh.bounds.center, Vector3.down, true, baseLine: false));
        
        direction = new Vector3(0, 0, 1);

        Utils.Direction[] h_directions = { Utils.Right, Utils.Left};
        Utils.Direction[] v_directions = {Utils.Up, Utils.Down};
        LotInfo BlockLot = LotFromParentBlock();

        LeftRightSubdivide(BlockLot, Utils.Up);
        LeftRightSubdivide(BlockLot, Utils.Down);
    }

    void LeftRightSubdivide(LotInfo lot, Utils.Direction v_dir) {
        if (lot.Frontage < 3.5f) return;

        LotInfo LotL = SubdivideBlock(lot, Utils.Left, v_dir);
        LotInfo LotR = SubdivideBlock(lot, Utils.Right, v_dir);
        
        LeftRightSubdivide(LotL, v_dir);
        LeftRightSubdivide(LotR, v_dir);
    }
    
    public Vector3 RayIntersect;
    public float rayDistance;
    public Vector3 boundsSize;
    public Vector3 boundsMax;
    public Vector3 boundsMin;
    public Vector3 boundsExtents;

    public List<Vector3> BlockRay(Vector3 origin, Vector3 dir, bool vertical = false, bool reciprocal = true, bool baseLine = false) {
        Vector3 point1 = origin;
        var points = new List<Vector3>();
        points.Add(origin);
        //Instantiate(marker, origin,Quaternion.identity,this.transform);
        Ray ray = new Ray(origin,dir*1000f);
        ray.origin = ray.GetPoint(20f);


        foreach (var pair in shiftedPairs) {
            Vector3 intersection;

            //i'm so so so so so sorry for this
            if (Math3d.AreLineSegmentsCrossing(ray.origin, origin, pair[0], pair[1])) {
                Math3d.LineLineIntersection(out intersection, ray.origin, ray.origin - origin, pair[0], pair[0] - pair[1]);
               
                if ((Mathf.Sign(dir.x) == -1 && intersection.x <= point1.x) 
                    || (Mathf.Sign(dir.x) == 1 && intersection.x >= point1.x) 
                    || (Mathf.Sign(dir.z) == 1 && vertical == true && intersection.z >= point1.z)
                    || (Mathf.Sign(dir.z) == -1 && vertical == true && intersection.z <= point1.z)) {

                    point1 = intersection;
                    //Debug.Log(point1);
                    if (baseLine) {
                        shiftedVerts.Add(intersection);
                    }
                    points.Add(point1);
                    //Instantiate(marker, point1, Quaternion.identity, this.transform);
                }
            }
        }
        
        return points;
    }

    LotInfo LotFromParentBlock() {
        LotInfo lot = new LotInfo(GetComponent<Block>(),shiftedVerts,direction,left: LeftAnchor, right: RightAnchor,parentBlock:true);
        LotInfo lot2 = new LotInfo(GetComponent<Block>(), shiftedVerts, direction, left: LeftAnchor, right: RightAnchor, parentBlock: true, parentLot: lot);
        return lot2;
    }

    LotInfo SubdivideBlock(LotInfo Lot,Utils.Direction h_dir,Utils.Direction v_dir) {
        List<Vector3> newPoints = new List<Vector3>();
        List<Vector3> lotVerts = Lot.LotVerts;
        Vector3 BlockCenter = self.mesh.bounds.center;
        Vector3 AnchorsH = (h_dir == Utils.Left) ? 
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, Vector3.left, vertical: false), Utils.Left):
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, Vector3.right, vertical: false), Utils.Right);
        
        Vector3 AnchorsV = (v_dir == Utils.Up) ? 
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, new Vector3(0, 0, 1), vertical: true), Utils.Up) :
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, new Vector3(0, 0, -1), vertical: true), Utils.Down);
        
        Utils.VectorList(lotVerts, AnchorsH);
        Utils.VectorList(lotVerts, AnchorsV);
        Utils.VectorList(lotVerts, BlockCenter);
        Utils.VectorList(lotVerts, Lot.Center);
        Utils.VectorList(lotVerts, self.shiftedVerts);

        Utils.VectorList(newPoints,Lot.Center);
        foreach (var vert in lotVerts) {
            if (h_dir == Utils.Left && vert.x <= Lot.Center.x && vert.x >= Lot.Left.x) {
                if (v_dir == Utils.Up && vert.z >= Lot.Center.z) {
                    Utils.VectorList(newPoints,vert);
                }
                if (v_dir == Utils.Down && vert.z <= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
            }
            if (h_dir == Utils.Right && vert.x <= Lot.Right.x && vert.x >= Lot.Center.x) {
                if (v_dir == Utils.Up && vert.z >= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
                if (v_dir == Utils.Down && vert.z <= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
            }
        }
        //Debug.Log(newPoints.Count);
      
        Vector3 left = (h_dir == Utils.Left) ? Lot.Left : Lot.Center;
        Vector3 right = (h_dir == Utils.Right) ? Lot.Right : Lot.Center;
        

        LotInfo newLot = new LotInfo(self,newPoints,direction,left: left,right: right,parentLot: Lot);
        
        GameObject LotObject = (GameObject)Instantiate(Resources.Load("Lot"));
        LotObject.GetComponent<Lot>().init(newLot);
        LotObject.transform.parent = transform;


        //Debug.Log(AnchorsH[AnchorsH.Count - 1]);
        foreach (var v in newLot.RoadFacingVerts) {
            //Instantiate(redMarker,v,Quaternion.identity,self.transform);
        }
        return newLot;
    }


    void SetMesh(List<Vector3> _verts, bool offset = false) {

        Triangulator tr = new Triangulator(Utils.V2dArray(_verts));
        int[] indices = tr.Triangulate();
        mesh.SetVertices(_verts);
        mesh.triangles = indices;

        mesh.RecalculateBounds();
        if (offset) {
            //GameObject indicator = Instantiate(Resources.Load("boundingPoint")) as GameObject;
            //indicator.transform.position = filter.mesh.bounds.center;
            //indicator.transform.parent = transform;
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
    public List<Vector3> points;
    public Vector3 Center;
    public Vector3 Left;
    public Vector3 Right;
    public Vector3 Direction;
    public LotInfo ParentLot;
    public float Frontage;
    public Segment RoadSegment;
    public List<Vector3> RoadFacingVerts;

    public LotInfo(Block block,List<Vector3> lotVerts, Vector3 direction,Vector3 left, Vector3 right, bool parentBlock = false, LotInfo parentLot = null) {
        ParentBlock = block;
        ParentLot = parentLot;
        LotVerts = lotVerts;
        Direction = direction;
        Left = left;
        Right = right;
        Center = (Left + Right)/2;
        Frontage = (right.x - left.x);

        List<Segment> segments = block.boundingSegments;
        RoadFacingVerts = new List<Vector3>();
        Vector3 _dir = direction;

        for (int i = 0; i < lotVerts.Count; i++) {
            for (int j = 0; j < segments.Count; j++) {
                Vector3 A2 = lotVerts[i] + _dir;
                if(Math3d.AreLineSegmentsCrossing(lotVerts[i], A2, segments[j].start(), segments[j].end())) {
                    RoadFacingVerts.Add(lotVerts[i]);
                }
            }
        }
        //Debug.Log(RoadFacingVerts.Count);
        //Frontage = Vector3.Distance(RoadFacingVerts[0],RoadFacingVerts[1]);
    }
}
