using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
//[System.Serializable]
public class Road : MonoBehaviour {

    public enum RoadType
    {
        Residential,
        Thoroughfare,
        Downtown
    }

    public RoadType roadType;

    /////////////////////////////////////

    public List<Node> nodes;
    public Vector3[] points_v;
    public List<Segment> segments;
    public List<SegmentCollider> segmentCols;
    public List<Lot> Lots;
    public Node StartNode;
    public Node EndNode;
    public List<Node[]> nodePairs;
    //public Dictionary<Segment, SegmentCollider> segmentToCol;

    public DictionaryOfSegmentAndSegmentCollider segmentToCol;
    //public List<Intersection> intersections;
    public LineRenderer line;
    public bool isActiveInEditor;
    public Node node_prefab;
    public SegmentCollider segmentPrefab;
    public string roadName;
    [SerializeField] Road self;

    // Use this for initialization
    void Awake () {

	}

    public void Init() {
        line = GetComponent<LineRenderer>();
        self = GetComponent<Road>();
        if (nodePairs == null) segmentCols = new List<SegmentCollider>();
        if (segments == null) segments = new List<Segment>();
        if (nodePairs == null) nodePairs = new List<Node[]>();
        if (nodesToSegment == null) nodesToSegment = new Dictionary<Node[], Segment>();
        if (segmentToCol == null) segmentToCol = new DictionaryOfSegmentAndSegmentCollider();
        AddNode();
        AddNode();
        StartNode = nodes[0]; EndNode = nodes[1];
        GenerateNodeSegments();
    }

    public void NameRoad(string n) {
        roadName = n;
        transform.name = n;
    }

    Dictionary<Node[],Segment> nodesToSegment;

    public void GenerateNodeSegments()
    {
        if (nodePairs.Count == 0) return;
        foreach (var pair in nodePairs) {
            if (!nodesToSegment.ContainsKey(pair)) {
                Segment segment = new Segment(pair[0], pair[1], GetComponent<Road>());
                nodesToSegment.Add(pair, segment);
                AddSegment(segment);
            }
        }
    }

    public void SplitSegment(Segment segment, Node node) {
        Node start = segment.startNode;
        Node end = segment.endNode;
        segment.endNode = node;
        Segment newSegment = new Segment(node, end, segment.road);
        AddSegment((newSegment));
    }

    public void AddSegment(Segment segment) {
        segments.Add(segment);
        SegmentCollider segmentCol = Instantiate(segmentPrefab,Vector3.zero,Quaternion.identity, transform);
        segmentToCol.Add(segment, segmentCol);
        if (!segmentCols.Contains(segmentCol)) segmentCols.Add(segmentCol);
        segmentCol.Init(segment, self);
    }

    public void RefreshSegments() {
        int i = 0;
        segments.ForEach(x => {
            x.startNode = nodes[i];
            x.endNode = nodes[i + 1];
            i++;
        });
    }

    public void AddLot(Lot L) {
        if(!Lots.Contains(L)) Lots.Add(L);
    }

    public void AssignLotAddresses() {
        int i = 1;
        segments.ForEach(x => {
            x.Lots.ForEach(j => {
                j.Address = i + " " + roadName;
                j.name = j.Address;
                i++;
            });
        });
    }

    public List<Node> OrderNodes() {
        List<Node> unordered = self.nodes;
        List<Node> ordered = new List<Node>();
        unordered.Remove(StartNode);
        unordered.Insert(0, StartNode);
        ordered.Add(StartNode);
        OrderNextNode(StartNode, ordered);
        return ordered;
    }

    private void OrderNextNode(Node previous, List<Node> ordered) {
        previous.adjNodes.ForEach(x => {
            if (self.nodes.Contains(x) && !ordered.Contains(x)) {
                ordered.Add(x);
                OrderNextNode(x, ordered);
                return;
            }
        });
    }

    public Segment GetSegmentFromStartNode(Node start) {
        Segment retSeg = null;
        foreach (var seg in segments) {
            if (seg.startNode == start) retSeg = seg;
            break;
        }
        return retSeg;
    }

    public void AddNode(Node node) {
        Node newNode = Instantiate(node_prefab,Vector3.zero,Quaternion.identity,transform);
        nodes.Add(newNode);
        NodeMap.instance.AddIntersection(newNode, self, self, coerce: true);
        nodePairs.Add(new Node[] { node, newNode });
        //newNode.transform.SetParent(transform);
        //Segment segment = new Segment(node, newNode, self);
        //AddSegment(segment);
        Selection.activeObject = newNode;
        //transform.parent.GetComponent<NodeMap>().PopulateNodeMap();
    }
    public void AddNode() {

        Node newNode = Instantiate(node_prefab,Vector3.zero,Quaternion.identity,transform);
        nodes.Add(newNode);
        NodeMap.instance.AddIntersection(newNode, self, self, coerce: true);
        if (nodes.Count > 1) {
            //newNode.transform.position = nodes[nodes.Count - 1].transform.position;
            nodePairs.Add(new Node[] { nodes[nodes.Count - 2], newNode });
            //Debug.Log("added pair");
        }
        //newNode.transform.SetParent(transform);
        Selection.activeObject = newNode;
        //transform.parent.GetComponent<NodeMap>().PopulateNodeMap();
    }

    public void RemoveNode(Node n) {
        nodes.Remove(n);
        DestroyImmediate(n.gameObject);
        Selection.activeObject = nodes[nodes.Count - 1];
    }

    // Update is called once per frame

    void Update () {
        UpdateRoad();
    }

    //[InspectorButton("UpdateRoad")]
    //public bool update;

    public void UpdateRoad() {
        if (isActiveInEditor) {
            transform.name = roadName;
            foreach (var n in nodes) {
                if (n.segments != null) n.roads = n.Roads();
            }

            if (Selection.activeGameObject == this.gameObject) {

            }
            segmentCols.ForEach(i => i.UpdateCollider());
        }
    }

    //[InspectorButton("AddCollider")]
    //public bool addCollider;
    void AddCollider() {
        for (int i = 0; i < segments.Count; i++) {
            if (!segmentToCol.ContainsKey(segments[i])) {
                SegmentCollider segmentObj = Instantiate(segmentPrefab,Vector3.zero,Quaternion.identity, transform);
                if (!segmentCols.Contains(segmentObj)) segmentCols.Add(segmentObj);
                segmentObj.Init(segments[i], self);
                segmentToCol.Add(segments[i], segmentObj);
            }
        }
    }
}
