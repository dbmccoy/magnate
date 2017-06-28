using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
//[System.Serializable]
public class Road : MonoBehaviour {

    public List<Node> nodes;
    public Vector3[] points_v;
    public List<Segment> segments;
    public List<SegmentCollider> segmentCols;
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
        //Debug.Log("now I won't null ref!");
        if (nodePairs.Count == 0) return;
        foreach (var pair in nodePairs) {
            if (!nodesToSegment.ContainsKey(pair)) {
                Segment segment = new Segment(pair[0], pair[1], GetComponent<Road>());
                nodesToSegment.Add(pair, segment);
                AddSegment(segment);
                /*foreach (var n in pair) {
                    if(n.segments == null || !n.segments.Contains(segment)) {
                        if (n.segments == null) n.segments = new List<Segment>();
                        n.segments.Add(segment);
                    }
                }*/
            }
        }
    }

    public void SplitSegment(Segment segment, Node node) {
        Node start = segment.startNode;
        Node end = segment.endNode;
        segment.endNode = node;
        //segment.nodes = new List<Node>{ start, node };
        /*segment.nodes.ForEach(x => {
        if (!x.segments.Contains(segment)) // ADD ADJACENT SEGMENTS
            x.segments.Add(segment);
            segment.nodes.ForEach(i => { // ADD ADJACENT NODES 
                if (!x.adjNodes.Contains(i)) x.adjNodes.Add(i);
                Debug.Log("sldfksdf");
            });
        });*/
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
            /*line.numPositions = nodes.Count;
            points_v = new Vector3[nodes.Count];

            for (int i = 0; i < nodes.Count; i++) {
                points_v[i] = nodes[i].transform.position;
            }
            //line.SetPositions(points_v);*/
            //GenerateNodeSegments();
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
