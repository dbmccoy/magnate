﻿using System.Collections;
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
            }
        }
    }

    public void SplitSegment(Segment segment, Node node) {
        Node start = segment.startNode;
        Node end = segment.endNode;
        segment.endNode = node;
        Segment newSegment = new Segment(node, end);
        AddSegment((newSegment));
    }

    public void AddSegment(Segment segment) {
        segments.Add(segment);
        SegmentCollider segmentCol = Instantiate(segmentPrefab, transform);
        segmentToCol.Add(segment, segmentCol);
        if (!segmentCols.Contains(segmentCol)) segmentCols.Add(segmentCol);
        segmentCol.Init(segment, self);
    }

    public void AddNode(Node node) {
        Debug.Log("add node");
        Node newNode = Instantiate(node_prefab);
        nodes.Add(newNode);
        NodeMap.instance.AddIntersection(newNode, self, self, coerce: true);
        nodePairs.Add(new Node[] { node, newNode });
        newNode.transform.SetParent(transform);
        Segment segment = new Segment(node, newNode, self);
        AddSegment(segment);
        Selection.activeObject = newNode;
        //transform.parent.GetComponent<NodeMap>().PopulateNodeMap();
    }
    public void AddNode() {

        Node newNode = Instantiate(node_prefab);
        nodes.Add(newNode);
        NodeMap.instance.AddIntersection(newNode, self, self, coerce: true);
        if (nodes.Count > 1) {
            //newNode.transform.position = nodes[nodes.Count - 1].transform.position;
            nodePairs.Add(new Node[] { nodes[nodes.Count - 2], newNode });
            //Debug.Log("added pair");
        }
        newNode.transform.SetParent(transform);
        Selection.activeObject = newNode;
        //transform.parent.GetComponent<NodeMap>().PopulateNodeMap();
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
            //line.SetPositions(points_v);
            //GenerateNodeSegments();
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