using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
[System.Serializable]
public class Road : MonoBehaviour {

    public List<Node> nodes;
    Vector3[] points_v;
    public List<Segment> segments;
    List<SegmentCollider> segmentCols;
    public List<Node[]> nodePairs;
    public Dictionary<Segment, SegmentCollider> segmentToCol;
    public List<Intersection> intersections;
    public LineRenderer line;
    public bool isActiveInEditor;
    public Node node_prefab;
    public SegmentCollider segmentPrefab;
    public string roadName;
    Road self;

    // Use this for initialization
    void Awake () {
        line = GetComponent<LineRenderer>();
        self = GetComponent<Road>();
        segmentCols = new List<SegmentCollider>();
        segments = new List<Segment>();
        nodePairs = new List<Node[]>();
        nodesToSegment = new Dictionary<Node[], Segment>();
        segmentToCol = new Dictionary<Segment, SegmentCollider>();
        AddNode();
        AddNode();
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
                segments.Add(segment);
                //add collider
                SegmentCollider segmentCol = Instantiate(segmentPrefab, transform);
                segmentToCol.Add(segment, segmentCol);
                if (!segmentCols.Contains(segmentCol)) segmentCols.Add(segmentCol);
                segmentCol.Init(segment, self);
            }
        }
    }

    public void AddNode() {
        Node newNode = Instantiate(node_prefab);
        nodes.Add(newNode);
        NodeMap.instance.AddIntersection(newNode, self, self);
        if (nodes.Count > 1) {
            //newNode.transform.position = nodes[nodes.Count - 1].transform.position;
            nodePairs.Add(new Node[] { nodes[nodes.Count - 2], newNode });
            //Debug.Log("added pair");
        }
        newNode.transform.SetParent(transform);
        Selection.activeObject = newNode;
        //transform.parent.GetComponent<NodeMap>().PopulateNodeMap();
    }

    private void NewSegment(Node node) {

    }

    public void RemoveNode(Node n) {
        nodes.Remove(n);
        DestroyImmediate(n.gameObject);
        Selection.activeObject = nodes[nodes.Count - 1];
        Debug.Log("removed node");
    }

    // Update is called once per frame

    void Update () {
        UpdateRoad();
    }

    [InspectorButton("UpdateRoad")]
    public bool update;

    public void UpdateRoad() {
        if (isActiveInEditor) {
            transform.name = roadName;

            if (Selection.activeGameObject == this.gameObject) {

            }
            line.numPositions = nodes.Count;
            points_v = new Vector3[nodes.Count];

            for (int i = 0; i < nodes.Count; i++) {
                points_v[i] = nodes[i].transform.position;
            }
            line.SetPositions(points_v);
            GenerateNodeSegments();
            segmentCols.ForEach(i => i.UpdateCollider());
        }
    }

    [InspectorButton("AddCollider")]
    public bool addCollider;
    void AddCollider() {
        for (int i = 0; i < segments.Count; i++) {
            if (!segmentToCol.ContainsKey(segments[i])) {
                SegmentCollider segmentObj = Instantiate(segmentPrefab, transform);
                if (!segmentCols.Contains(segmentObj)) segmentCols.Add(segmentObj);
                segmentObj.Init(segments[i], self);
                segmentToCol.Add(segments[i], segmentObj);
            }
        }
    }
}
