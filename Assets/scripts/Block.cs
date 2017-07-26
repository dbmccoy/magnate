using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

//[System.Serializable]
[ExecuteInEditMode]

public class Block: MonoBehaviour {

    public List<Lot> Lots;
    public List<Segment> boundingSegments = new List<Segment>();
    List<Node> nodes = new List<Node>();
    public List<Road> boundingRoads;
    public List<Vector3> verts = new List<Vector3>();
    public List<Vector3> shiftedVerts = new List<Vector3>();
    public Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer renderer;
    public GameObject marker;
    public GameObject redMarker;
    private Block self;
    
    public void InitBlock(List<Segment> _boundingSegments) {
        marker = (GameObject)Resources.Load("boundingPoint");
        redMarker = (GameObject)Resources.Load("redMarker");
        self = GetComponent<Block>();
        direction = Vector3.forward;
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
        renderer.enabled = false;
    }

    public List<Vector3> debugPoints = new List<Vector3>();
    public float minimumFrontage;
    public Vector3 direction;
    public Vector3 LeftAnchor;
    public Vector3 RightAnchor;
    public Vector3 UpAnchor;
    public Vector3 DownAnchor;

    public float X_angle;
    public Vector3 RayVectorH;
    public Vector3 RayVectorV;
    public float RayVectorAngleH;
    public float RayVectorAngleV;
    public float AngleDown;
    public float AngleUp;
    public float AngleLeft;
    public float AngleRight;
    [InspectorButton("AngleCheck")]
    public bool angleCheck;

    public void AngleCheck() {
        Ray ray = new Ray(BlockCenter, Vector3.forward  * 1000f);
        Ray ray2 = new Ray(BlockCenter, Vector3.back * 1000f);
        Ray RayL = new Ray(BlockCenter, Vector3.left * 1000f);
        Ray RayR = new Ray(BlockCenter, Vector3.right * 1000f);

        Debug.DrawLine(ray.origin,ray.GetPoint(10));
        AngleDown = 0;
        AngleUp = 0;
        AngleRight = 0;
        AngleRight = 0;
        float top_distance = 0;
        float bot_distance = 0;
        float left_distance = 0;
        float right_distance = 0;
        Vector3 top_intersect;
        Vector3 bot_intersect;
        foreach (var segment in boundingSegments) {  //FIND TOP AND BOTTOM INTERSECTION POINTS, CALCULATE CENTER
            if (Math3d.AreLineSegmentsCrossing(ray.origin, ray.GetPoint(20f), segment.start(), segment.end())) {
                AngleUp = segment.Angle();
                //X_angle_top = Math3d.SignedVectorAngle(segment.end() - segment.start(), Vector3.right, Vector3.forward);
                Math3d.LineLineIntersection(out top_intersect, ray.origin, ray.direction, segment.start(), segment.vector());
                top_distance = Vector3.Distance(ray.origin,top_intersect);
            }
            if (Math3d.AreLineSegmentsCrossing(ray2.origin, ray2.GetPoint(20f), segment.start(), segment.end())) {
                AngleDown = segment.Angle();
                //X_angle_top = (Mathf.Sign(X_angle_top) == 1) ? X_angle_top - 90f : X_angle_top +90f;
                Math3d.LineLineIntersection(out bot_intersect, ray.origin, ray2.direction, segment.start(), segment.vector());
                bot_distance = Vector3.Distance(ray.origin, bot_intersect);
            }
            if (Math3d.AreLineSegmentsCrossing(RayL.origin, RayL.GetPoint(20f), segment.start(), segment.end())) {
                AngleLeft = segment.angleV();
                Math3d.LineLineIntersection(out bot_intersect, ray.origin, ray2.direction, segment.start(), segment.vector());
                left_distance = Vector3.Distance(ray.origin, bot_intersect);
            }
            if (Math3d.AreLineSegmentsCrossing(RayR.origin, RayR.GetPoint(20f), segment.start(), segment.end())) {
                AngleRight = segment.angleV();
                Math3d.LineLineIntersection(out bot_intersect, ray.origin, ray2.direction, segment.start(), segment.vector());
                right_distance = Vector3.Distance(ray.origin, bot_intersect);
            }
        }
        float distance = bot_distance - top_distance;
        //float topAdj = BlockCenter.z + top_distance;
        //float botAdj = BlockCenter.z - bot_distance;
        BlockCenter = BlockCenter - new Vector3(0, 0, distance/2);
        RayVectorAngleH = (AngleDown + AngleUp) / 2;

        if (Mathf.Abs(RayVectorAngleH) > 10) RayVectorAngleV = (AngleLeft + AngleRight) / 2;
        else RayVectorAngleV = 90;

        //if (Mathf.Abs(RayVectorAngleH) > 10) RayVectorAngleH = RayVectorAngleV - 90;


        if (RayVectorAngleV < 0) RayVectorV = Quaternion.Euler(0,RayVectorAngleV,0)*Vector3.right;
        else RayVectorV = Quaternion.Euler(0, RayVectorAngleV, 0) * Vector3.left;

        RayVectorH = Quaternion.Euler(0,RayVectorAngleH,0)*Vector3.right;
        if(Mathf.Abs(RayVectorAngleH) >= 20f) {
            //RayVectorV = Quaternion.Euler(0,-90f,0) * RayVectorH;
        }
    }

    [InspectorButton("BlockRayButton")]
    public bool blockRay;

    public void BlockRayButton() {
        BlockCenter = mesh.bounds.center;
        //Instantiate(marker, BlockCenter, Quaternion.identity);
        bool isVertical = false;
        AngleCheck();

        RightAnchor = Utils.ReturnMaximalVector(BlockRay(BlockCenter, RayVectorH, isVertical, baseLine: true),Utils.Right);
        LeftAnchor = Utils.ReturnMaximalVector(BlockRay(BlockCenter, -RayVectorH, isVertical, baseLine: true),Utils.Left);
        UpAnchor = Utils.ReturnMaximalVector(BlockRay(BlockCenter, RayVectorV, isVertical, baseLine: true), Utils.Up);
        DownAnchor = Utils.ReturnMaximalVector(BlockRay(BlockCenter, -RayVectorV, isVertical, baseLine: true), Utils.Down);
        
        Utils.Direction[] h_directions = { Utils.Right, Utils.Left};
        Utils.Direction[] v_directions = {Utils.Up, Utils.Down};
        LotInfo BlockLot = LotFromParentBlock();

        LeftRightSubdivide(BlockLot, Utils.Up);
        LeftRightSubdivide(BlockLot, Utils.Down);
        NeighborLots();
    }

    void LeftRightSubdivide(LotInfo lot, Utils.Direction v_dir) {
        if (lot.Frontage < 2.7f) return;

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
        Ray ray = new Ray(origin,dir*1000f);
        ray.origin = ray.GetPoint(20f);

        boundingSegments.ForEach(x =>
        {
            Vector3 intersection;

            if (Math3d.AreLineSegmentsCrossing(ray.origin, origin, x.StartWithOffset(origin), x.EndWithOffset(origin)))
            {
                Math3d.LineLineIntersection(out intersection, ray.origin, ray.origin - origin, x.StartWithOffset(origin), x.vector());

                if ((Mathf.Sign(dir.x) == -1 && intersection.x <= point1.x) //WHAT THE FUCK IS THIS
                    || (Mathf.Sign(dir.x) == 1 && intersection.x >= point1.x)
                    || (Mathf.Sign(dir.z) == 1 && vertical == true && intersection.z >= point1.z)
                    || (Mathf.Sign(dir.z) == -1 && vertical == true && intersection.z <= point1.z))
                {

                    point1 = intersection;
                    //Debug.Log(point1);
                    if (baseLine)
                    {
                        //shiftedVerts.Add(intersection);
                    }
                    points.Add(point1);
                    //Instantiate(marker, point1, Quaternion.identity, this.transform);
                }
            }
        });
        foreach (var pair in shiftedPairs) {

        }
        
        return points;
    }

    LotInfo LotFromParentBlock() {
        LotInfo lot = new LotInfo(GetComponent<Block>(),shiftedVerts,direction,left: LeftAnchor, right: RightAnchor,parentBlock:true);
        LotInfo lot2 = new LotInfo(GetComponent<Block>(), shiftedVerts, direction, left: LeftAnchor, right: RightAnchor, parentBlock: true, parentLot: lot);
        return lot2;
    }

    public Vector3 BlockCenter;

    LotInfo SubdivideBlock(LotInfo Lot,Utils.Direction h_dir,Utils.Direction v_dir) {
        List<Vector3> newPoints = new List<Vector3>();
        List<Vector3> lotVerts = new List<Vector3>();
        lotVerts.AddRange(Lot.LotVerts);
        Vector3 rayVectorV = (v_dir == Utils.Up) ? RayVectorV : -RayVectorV;

        Vector3 AnchorsH = (h_dir == Utils.Left) ? 
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, -RayVectorH, vertical: false), Utils.Left):
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, RayVectorH, vertical: false), Utils.Right);
        
        Vector3 AnchorsV = (v_dir == Utils.Up) ? 
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, RayVectorV, vertical: true), Utils.Up) :
                Utils.ReturnMaximalVector(BlockRay(Lot.Center, -RayVectorV, vertical: true), Utils.Down);

        Vector3 left = (h_dir == Utils.Left) ? Lot.Left : Lot.Center;
        Vector3 right = (h_dir == Utils.Right) ? Lot.Right : Lot.Center;

        Utils.VectorList(lotVerts, AnchorsH);
        Utils.VectorList(lotVerts, AnchorsV);
        Utils.VectorList(lotVerts, Lot.Center);
        Utils.VectorList(lotVerts, self.shiftedVerts);
       // Utils.VectorList(lotVerts, BlockCenter);

        Utils.VectorList(newPoints, left);
        Utils.VectorList(newPoints, right);
        Utils.VectorList(newPoints, Utils.ReturnMaximalVector(BlockRay(left, rayVectorV), v_dir));
        Utils.VectorList(newPoints, Utils.ReturnMaximalVector(BlockRay(right, rayVectorV), v_dir));


        foreach (var vert in lotVerts) {
            if (h_dir == Utils.Left && vert.x <= Lot.Center.x && vert.x >= (Lot.Left.x - Vector3.Distance(Lot.Center,Lot.Left))) {
                if (v_dir == Utils.Up && vert.z >= Lot.Center.z) {
                    Utils.VectorList(newPoints,vert);
                }
                if (v_dir == Utils.Down && vert.z <= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
            }
            if (h_dir == Utils.Right && vert.x <= Lot.Right.x + Vector3.Distance(Lot.Center, Lot.Right)  && vert.x >= Lot.Center.x ) {
                if (v_dir == Utils.Up && vert.z >= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
                if (v_dir == Utils.Down && vert.z <= Lot.Center.z) {
                    Utils.VectorList(newPoints, vert);
                }
            }
        }
        //Debug.Log(newPoints.Count);
      


        LotInfo newLot = new LotInfo(self,newPoints,direction,left: left,right: right,parentLot: Lot);
        
        if(newLot.Frontage < 2.7) {
            GameObject LotObject = (GameObject)Instantiate(Resources.Load("Lot"));
            LotObject.GetComponent<Lot>().init(newLot);
            LotObject.transform.parent = transform;
        }


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
        //foreach (var vert in verts) {
        //    Vector3 v = Vector3.ClampMagnitude((vert - center), (vert - center).magnitude * .8f);
        //    newVerts.Add(center + v);
        //}

        boundingSegments.ForEach(x =>
        {
            boundingSegments.ForEach(j =>
            {
                if (Math3d.AreLineSegmentsCrossing(x.StartWithOffset(center), x.EndWithOffset(center), j.StartWithOffset(center), j.EndWithOffset(center)))
                {
                    Vector3 hit;
                    if (Math3d.LineLineIntersection(out hit, x.StartWithOffset(center), x.vector(), j.StartWithOffset(center), j.vector()))
                    {
                        newVerts.Add(hit);
                    }
                }
            });
        });

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

    public void NeighborLots()
    {
        Lots.ForEach(x =>
        {
            List<Edge> sideEdges = new List<Edge>();
            List<Vector3> frontPoints = new List<Vector3> { x.frontend, x.frontstart};
            x.edges.ForEach(j =>
            {
                if ((frontPoints.Contains(j.start()) || frontPoints.Contains(j.end())) && j != x.frontEdge )
                {
                    if (!sideEdges.Contains(j))
                    {
                        sideEdges.Add(j);
                    }
                }
            });
            x.edges.ForEach(j =>
            {
                Lots.ForEach(i =>
                {
                    i.edges.ForEach(e =>
                    {
                        if (j.EqualTo(e))
                        {
                            if(i != x && !x.adjacentLots.ContainsKey(i)) x.adjacentLots.Add(i,j);
                        }
                    });
                });
            });
            x.adjacentLots.Values.ToList().ForEach(v =>
            {
                if (sideEdges.Contains(v)) sideEdges.Remove(v);
            });
            x.cornerEdge = sideEdges.FirstOrDefault();
        });
    }

}

//public class LotInfo {
//    public Block ParentBlock;
//    public Utils.Direction HorizontalDirection;
//    public Utils.Direction VerticalDirection;
//    public List<Vector3> LotVerts;
//    public List<Vector3> points;
//    public Vector3 Center;
//    public Vector3 Left;
//    public Vector3 Right;
//    public Vector3 Direction;
//    public Vector3 LotCenter;
//    public LotInfo ParentLot;
//    public float Frontage;
//    public Segment RoadSegment;
//    public List<Vector3> RoadFacingVerts;
//    public Vector3 RoadPoint;

//    public LotInfo(Block block,List<Vector3> lotVerts, Vector3 direction,Vector3 left, Vector3 right, bool parentBlock = false, LotInfo parentLot = null) {
//        ParentBlock = block;
//        ParentLot = parentLot;
//        LotVerts = lotVerts;
//        Direction = direction;
//        Left = left;
//        Right = right;
//        Center = (Left + Right)/2;
//        Frontage = (right.x - left.x);

//        List<Segment> segments = block.boundingSegments;
//        RoadFacingVerts = new List<Vector3>();
//        LotCenter = Utils.AverageVectors(lotVerts);
//        Vector3 _dir = new Vector3(0,0, (LotCenter - ParentBlock.BlockCenter).z).normalized * 10f;
//        Direction = _dir;
//        if(_dir == Vector3.zero) {
//            //Debug.Log("center " + LotCenter + " parent " + ParentBlock.BlockCenter);
//        }


//        for (int i = 0; i < lotVerts.Count; i++) {
//            for (int j = 0; j < segments.Count; j++) {
//                Vector3 A2 = lotVerts[i] + _dir;
//                if (Math3d.AreLineSegmentsCrossing(LotCenter, LotCenter + _dir, segments[j].start(), segments[j].end())) {
//                    RoadSegment = segments[j];
//                    if(Math3d.LineLineIntersection(out RoadPoint,
//                        LotCenter, _dir, segments[j].start(), segments[j].vector())) return;
//                }
//            }
//        }
//        //Debug.Log(RoadFacingVerts.Count);
//        //Frontage = Vector3.Distance(RoadFacingVerts[0],RoadFacingVerts[1]);
//    }
//}
