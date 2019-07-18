using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Lot : MonoBehaviour, IAsset {

    //IAsset implementation
    public string Name { get; set; }
    public Entity OwningEntity { get; set; }
    public string Address;
    public float SquareFeet;
    public Neighborhood Neighborhood { get; set; }
    public string Class { get; set; }
    public float ValueToOwner { get; set; }

    public BuildingDesign Design;

    public Building Building = null;

    public List<Tuple<Use, float>> Uses = new List<Tuple<Use, float>>();
    
    float lastSalePrice;
    public float LastSalePrice {
        get {
            if (lastSalePrice == 0) {
                lastSalePrice = GameManager.Instance.BaseLandCost * SquareFeet;
            }
            return lastSalePrice;
        }
        set => lastSalePrice = value; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public void SetNeighborhood(Neighborhood n) {
        Neighborhood = n;
    }

    public void SetDesign(BuildingDesign d) {
        Design = d;
    }

    public bool CanBuild(Building b) {
        //TODO: make this handle additions and multi building lots

        if (Building == null) {
            return true;
        }
        else return false;
    }

    public LotUpdateEvent OnLotUpdate = new LotUpdateEvent();

    public float GetValue()
    {
        return block.Lots.Select(x => x.LastSalePrice / x.SquareFeet).Average() * SquareFeet;
    }

    public float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }

    public void GrantActionsTo(Person p) {
        if (p.GetComponent<RealEstateSensor>() == null) {
            p.AddComponent<RealEstateSensor>();
        }

    }

    public void RevokeActionsFrom(Person p) {

    }

    public void Transfer(Entity to)
    {
        if(to != OwningEntity) {
            OwningEntity.DivestAsset(this);
        }
        OwningEntity = to;
        OwningEntity.AcquireAsset(this);
        OwningEntity.AcquireAsset(Building);
    }

    //UI*UI*UI*UI

    private string contextPrint;

    public void SetContextPrint(string s) {
        contextPrint = s;
    }

    public void ResetContextPrint() {
        contextPrint = Address;
    }

    public string ContextPrint() {
        if(contextPrint == "") {
            contextPrint = Address;
        }
        return contextPrint;
    }

    //UI*UI*UI*UI


    public Road road;
    public Block block;
    public Segment segment;

    [HideInInspector] public List<Vector3> verts;
    [HideInInspector] public List<Vector3> buildableVerts;
    [HideInInspector] public Edge frontEdge;
    [HideInInspector] public Edge cornerEdge;
    [HideInInspector] public Dictionary<Lot,Edge> adjacentLots = new Dictionary<Lot, Edge>();
    [HideInInspector] public List<Edge> edges;
    [HideInInspector] public List<Vector3> points = new List<Vector3>();
    [HideInInspector] public Vector3 center;
    [HideInInspector] public Vector3 left;
    [HideInInspector] public Vector3 right;
    [HideInInspector] public MeshCollider col;
    [HideInInspector] public string horizontal;
    [HideInInspector] public LotInfo info;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Vector3 RoadPoint;
    [HideInInspector] public Lot self;

    //segment ordering variables
    [HideInInspector] public bool isLeftOfSegVector;
    [HideInInspector] public float angleToSegStart;

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
            //GenZone();
            ReturnBuildableVerts();
            //Bug.Mark(frontEdge.start()); Bug.Mark(frontEdge.end());
        }
    }

    float meshArea() {
        float temp = 0;
        int i = 0;
        for (; i < verts.Count; i++) {
            if (i != verts.Count - 1) {
                float mulA = verts[i].x * verts[i + 1].z;
                float mulB = verts[i + 1].x * verts[i].z;
                temp = temp + (mulA - mulB);
            }
            else {
                float mulA = verts[i].x * verts[0].z;
                float mulB = verts[0].x * verts[i].z;
                temp = temp + (mulA - mulB);
            }
        }
        temp *= 0.5f;
        return Mathf.Abs(temp);
    }

    [HideInInspector] public Vector3 frontstart, frontend;

    //segment ordering vars

    [HideInInspector] public List<float> angles;

    private void OnMouseDown() {
        Debug.Log("click");
    }

    [HideInInspector] public float segDistance; //DEBUG ONLY

    public float DistanceToSegStart() {
        try
        {
            segDistance = Vector3.Distance(RoadPoint, segment.startNode.pos());
            return segDistance;
        }
        catch
        {
            Debug.Log(name);
            return 0f;
        }
    }

    public void ReturnBuildableVerts()
    {
        float f = info.Zoning.FrontSetback;
        float s = info.Zoning.SideSetback;

        edges.ForEach(x =>
        {
            edges.ForEach(j =>
            {

                Vector3 hit;
                if (Math3d.LineLineIntersection(out hit, x.start(info.LotCenter,f), x.vector(), j.start(), j.vector()))
                {
                    //Instantiate(Utils.Marker(), hit, Quaternion.identity);
                }
            });
        });

    }

    public void Awake()
    {
        GenZone();
        GameManager.Instance.Lots.Add(this);
    }

    public float DistanceTo(Lot to) {
        return Vector3.Distance(this.center, to.center);
    }

    public float BuildableSquareFeet() {
        return SquareFeet * 10f; //calc buildable area * height limit for zone/tech
    }

    public Zone.ZoneClass z_Class;

    public void SetZone(Zone _zone)
    {
        if(_zone.zone == Zone.ZoneClass.CS)
        {
            z_Class = _zone.zone;
            self.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/red");
        }
        if (_zone.zone == Zone.ZoneClass.R5)
        {
            z_Class = _zone.zone;
            self.GetComponent<MeshRenderer>().material = (Material)Resources.Load("materials/yellow");
        }
    }

    public void GenZone()
    {
        if(road.roadType == Road.RoadType.Thoroughfare)
        {
            //Debug.Log(ZoneManager.i.CS.zone);
            SetZone(ZoneManager.i.CS);
        }
        if (road.roadType == Road.RoadType.Residential)
        {
            SetZone(ZoneManager.i.R5);
        }
        else
        {
            SetZone(ZoneManager.i.R5);
        }
        Name = Address;
    }

    public void Build() {
        GameObject building = (GameObject)Instantiate(Resources.Load("buildings/house_test_1"), center, Quaternion.LookRotation(direction),transform);
    }

    [HideInInspector] public Vector3[] MeshVerts;
    [HideInInspector] public int[] triangles;
    [HideInInspector] public Vector2[] arr;

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

        SquareFeet = meshArea() * 600f;
    }
}

[Serializable]
public class lot_Params
{
    public int sq_ft;
    public List<Vector3> bounds;
    public List<Vector3> buildableBounds;
    public Zone zone_Info;

    public lot_Params()
    {

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

    public Vector3 vector()
    {
        return end() - start();
    }

    public Vector3 vector(Vector3 start, Vector3 end)
    {
        return end - start;
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

public class LotUpdateEvent : UnityEvent<Lot> {

}
