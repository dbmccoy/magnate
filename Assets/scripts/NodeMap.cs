using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

//[System.Serializable]
public class NodeMap : MonoBehaviour {

    public Road[] roads;
    public List<Segment> nodeSegments;
    public List<Node> nodes;
    public List<string> possibleStreetNames;
    public List<string> currentStreetNames;
    public Node node_prefab;
    //public List<NewNode> allNodes;

    // Use this for initialization
    void Start () {
	}

    private static NodeMap _nodeMap;

    public static NodeMap instance
    {
        get { return _nodeMap ?? (_nodeMap = GameObject.Find("Roads").GetComponent<NodeMap>()); }
    }
    [SerializeField]
    private List<Vector3> UpdatedIntersectionPositions;

    public void AddIntersection(Node node, Road road1, Road road2, Segment seg1 = null, Segment seg2 = null, bool coerce = false, Node.Type type = Node.Type.turn) {
        UpdatedIntersectionPositions.Add(node.pos());
        if (type == Node.Type.intersection && CheckForIntersection(node.pos())) return;
        //Intersection intersection = Instantiate(intersectionPrefab, transform);
        
        //intersection.transform.position = node.pos();
        node.transform.name = road1.roadName + " and " + road2.roadName;
        //intersection.transform.parent = node.transform;
        nodes.Add(node);
        if (!road1.nodes.Contains(node)) road1.nodes.Add(node);
        if (!road2.nodes.Contains(node)) road2.nodes.Add(node);
        if (seg1 != null) seg1.AddIntersection(node);
        if (seg2 != null) seg2.AddIntersection(node);
        node.Init(new List<Segment> { seg1, seg2 });
    }


    public void RemoveIntersection(Node node) {
        nodes.Remove(node);
        DestroyImmediate(node.gameObject);
    }

    [InspectorButton("GenerateBlockRoads")]
    public bool CreateBlock;

    public void GenerateBlockRoads() {
        Road road1 = ReturnRoad();
        road1.nodePairs[0][0].transform.position = new Vector3(-6, 0, 5);
        road1.nodePairs[0][1].transform.position = new Vector3(6, 0, 5);
        Road road2 = ReturnRoad();
        road2.nodePairs[0][0].transform.position = new Vector3(-6, 0, -5);
        road2.nodePairs[0][1].transform.position = new Vector3(6, 0, -5);
        Road road3 = ReturnRoad();
        road3.nodePairs[0][0].transform.position = new Vector3(-5, 0, 6);
        road3.nodePairs[0][1].transform.position = new Vector3(-5, 0, -6);
        Road road4 = ReturnRoad();
        road4.nodePairs[0][0].transform.position = new Vector3(5, 0, 6);
        road4.nodePairs[0][1].transform.position = new Vector3(5, 0, -6);

        StartCoroutine(IntersectionsAtEndOfFrame());
    }

    IEnumerator IntersectionsAtEndOfFrame() {
        yield return new WaitForEndOfFrame();
        PopulateNodeMap();
    }

    public void AddRoad() {
        GameObject newRoad = Instantiate(Resources.Load("road"), transform) as GameObject;
        newRoad.GetComponent<Road>().Init();
        Selection.activeObject = newRoad;
        RoadName(newRoad.GetComponent<Road>());
    }

    public Node NewNode(Vector3 pos) {
        Node newNode = Instantiate(node_prefab);
        newNode.transform.position = pos;
        return newNode;
    }

    Road ReturnRoad() {
        GameObject newRoad = Instantiate(Resources.Load("road"), transform) as GameObject;
        newRoad.GetComponent<Road>().Init();
        Selection.activeObject = newRoad;
        RoadName(newRoad.GetComponent<Road>());
        return newRoad.GetComponent<Road>();
    }

    void RoadName(Road road) {
        int i = Random.Range(0, possibleStreetNames.Count - 1);
        string roadname = "default";
        while (!currentStreetNames.Contains(possibleStreetNames[i])) {
            roadname = possibleStreetNames[i];
            currentStreetNames.Add(roadname);
            i = Random.Range(0, possibleStreetNames.Count - 1);
        }
       // Debug.Log("road named " + roadname);
        road.NameRoad(roadname);
    }

    void CleanUpIntersections() {
        List<Node> intersectionsToRemove = new List<Node>();
        foreach (var item in nodes) {
            if (!UpdatedIntersectionPositions.Contains(item.transform.position)) {
                intersectionsToRemove.Add(item);
            }
        }
        foreach (var item in intersectionsToRemove) {
            if(item.type == global::Node.Type.intersection) {
                RemoveIntersection(item);
            }
        }
        intersectionsToRemove.Clear();
        UpdatedIntersectionPositions.Clear();
    }

    bool CheckForIntersection(Vector3 pos) {
        if (nodes.Count == 0) return false;
        foreach (var item in nodes) {
            if(item.transform.position == pos) {
                //Debug.Log("intersection already exists");
                return true;
            }
        }
        //Debug.Log("intersection position open");
        return false;
    }

    public void PopulateNodeMap() {
        roads = GetComponentsInChildren<Road>();
        Vector3 road1start, road1end, road2start, road2end, road1vec, road2vec;
        nodeSegments = new List<Segment>();

        for (int i = 0; i < roads.Length; i++) {
            if (roads.Length > 1) {
                for (int j = 0; j < roads[i].segments.Count; j++) {
                    for (int k = 0; k < roads.Length; k++) {
                        if (roads[k] != roads[i]) {
                            for (int t = 0; t < roads[k].segments.Count; t++) {
                                road1start = roads[i].segments[j].start();
                                road1end = roads[i].segments[j].end();
                                road1vec = roads[i].segments[j].vector();
                                if (!nodeSegments.Contains(roads[i].segments[j])) {
                                    nodeSegments.Add(roads[i].segments[j]);
                                    Debug.DrawLine(roads[i].segments[j].start(), roads[i].segments[j].end(), Color.red);
                                }
                                road2start = roads[k].segments[t].start();
                                road2end = roads[k].segments[t].end();
                                road2vec = roads[k].segments[t].vector();

                                if (Math3d.AreLineSegmentsCrossing(road1start, road1end, road2start, road2end)) {
                                    Vector3 intersection;
                                    Math3d.LineLineIntersection(out intersection, road1start, road1vec, road2start, road2vec);
                                    if (CheckForIntersection(intersection) == false) {
                                        Node node = NewNode(intersection);
                                        node.transform.parent = roads[i].transform;
                                        Segment seg1 = roads[i].segments[j];
                                        Segment seg2 = roads[k].segments[t];
                                        AddIntersection(node, roads[i], roads[k], seg1, seg2, type: Node.Type.intersection);
                                        roads[i].SplitSegment(seg1,node);
                                        roads[k].SplitSegment(seg2,node);
                                    } 
                                }
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log(nodeSegments.Count + " road nodes in NodeMap");
        CleanUpIntersections();
    }



    [InspectorButton("OnButtonClicked")]
    public bool update;

    private void OnButtonClicked() {
        PopulateNodeMap();
    }

    [InspectorButton("AddRoadButton")]
    public bool addRoad;

    private void AddRoadButton() {
        AddRoad();
    }

    [InspectorButton("ResetRoads")]
    public bool reset;



    private void ResetRoads() {
        for (int i = 0; i < transform.childCount; i++) {
            DestroyImmediate(transform.GetChild(i).gameObject);
            nodes.Clear();
            currentStreetNames.Clear();
            nodeSegments.Clear();
            roads = new Road[0];
        }
    }
}
