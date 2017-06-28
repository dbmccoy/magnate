using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Linq;

//[System.Serializable]
[ExecuteInEditMode]
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
        if (seg1 != null && seg2 != null) node.Init(new List<Segment> { seg1, seg2 });
        else node.Init();
    }


    public void RemoveIntersection(Node node) {
        nodes.Remove(node);
        DestroyImmediate(node.gameObject);
    }

    [InspectorButton("GenerateBlockRoads")]
    public bool CreateBlock;

    public void GenerateBlockRoads() {
        Road road1 = ReturnRoad();
        road1.nodePairs[0][0].transform.position = new Vector3(-20, 0, 5);
        road1.nodePairs[0][1].transform.position = new Vector3(20, 0, 5);
        Road road2 = ReturnRoad();
        road2.nodePairs[0][0].transform.position = new Vector3(-20, 0, -5);
        road2.nodePairs[0][1].transform.position = new Vector3(20, 0, -5);
        Road road3 = ReturnRoad();
        road3.nodePairs[0][0].transform.position = new Vector3(-5, 0, 20);
        road3.nodePairs[0][1].transform.position = new Vector3(-5, 0, -20);
        Road road4 = ReturnRoad();
        road4.nodePairs[0][0].transform.position = new Vector3(5, 0, 20);
        road4.nodePairs[0][1].transform.position = new Vector3(5, 0, -20);
        Road road5 = ReturnRoad();
        road5.nodePairs[0][0].transform.position = new Vector3(15, 0, 20);
        road5.nodePairs[0][1].transform.position = new Vector3(15, 0, -20);
        Road road6 = ReturnRoad();
        road6.nodePairs[0][0].transform.position = new Vector3(-15, 0, 20);
        road6.nodePairs[0][1].transform.position = new Vector3(-15, 0, -20);
        Road road7 = ReturnRoad();
        road7.nodePairs[0][0].transform.position = new Vector3(20, 0, 15);
        road7.nodePairs[0][1].transform.position = new Vector3(-20, 0, 15);
        Road road8 = ReturnRoad();
        road8.nodePairs[0][0].transform.position = new Vector3(20, 0, -15);
        road8.nodePairs[0][1].transform.position = new Vector3(-20, 0, -15);


        StartCoroutine(IntersectionsAtEndOfFrame());
        Debug.Log("now");
    }

    public List<Road> SortRoads(bool EW) { //NOT DONE
        List<Road> roadList = roads.ToList();
        Dictionary<Road, float> roadDict = new Dictionary<Road, float>();
        foreach (var road in roadList) {
            List<float> xPoints = new List<float>();
            road.nodes.ForEach(x => xPoints.Add(x.pos().x));
            roadDict.Add(road, xPoints.Sum() / xPoints.Count);
        }
        SortedDictionary<Road, float> roadDictSorted = new SortedDictionary<Road, float>(roadDict);
        roadList = roadDictSorted.Keys.ToList();
        return roadList;
    }

    IEnumerator IntersectionsAtEndOfFrame() {
        Debug.Log("start");
        yield return new WaitForEndOfFrame();
        Debug.Log("go");
        PopulateNodeMap();
    }

    public void AddRoad() {
        GameObject newRoad = Instantiate(Resources.Load("road"),Vector3.zero,Quaternion.identity, transform) as GameObject;
        newRoad.GetComponent<Road>().Init();
        Selection.activeObject = newRoad;
        RoadName(newRoad.GetComponent<Road>());
    }

    public Node NewNode(Vector3 pos) {
        Node newNode = Instantiate(node_prefab,pos,Quaternion.identity);
        //newNode.transform.position = pos;
        return newNode;
    }

    Road ReturnRoad() {
        GameObject newRoad = Instantiate(Resources.Load("road"),Vector3.zero,Quaternion.identity, transform) as GameObject;
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
        road.nodes.ForEach(x => x.roads.Add(road));
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
        SetNodeSegments();
        roads.ToList().ForEach(x => x.nodes = x.OrderNodes());
    }

    [InspectorButton("SetNodeSegments")]
    public bool SetNodeSeg;

    public void SetNodeSegments() {
        nodes.ForEach(x => x.segments = new List<Segment>());
        nodeSegments.ForEach(x => {
            if(!x.startNode.segments.Contains(x)) x.startNode.segments.Add(x);
            if (!x.endNode.segments.Contains(x)) x.endNode.segments.Add(x);
        });
        GenerateAdjNodes();
    }

    public void GenerateAdjNodes() {
        nodes.ForEach(x => {
            x.adjNodes = new List<Node>();
            x.segments.ForEach(i => {
                if (i.startNode != x && !x.adjNodes.Contains(i.startNode)) x.adjNodes.Add(i.startNode);
                if (i.endNode != x && !x.adjNodes.Contains(i.endNode)) x.adjNodes.Add(i.endNode);
            });
        });
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
        //BlockMap.instance.blocks.ForEach(x => DestroyImmediate(x.gameObject));
        //BlockMap.instance.blocks.Clear();
        nodes.Clear();
        roads.ToList().ForEach(x => DestroyImmediate(x.gameObject));
        //nodes.ForEach(x => DestroyImmediate(x.gameObject));
        currentStreetNames.Clear();
        nodeSegments.Clear();

        for (int i = 0; i < transform.childCount; i++) {
            //DestroyImmediate(transform.GetChild(i).gameObject);
        }

        roads = new Road[0];

    }

    [InspectorButton("AssignLotAddresses")]
    public bool LotAddresses;

    public void AssignLotAddresses() {
        roads.ToList().ForEach(x => {
            int number = 1;
            x.nodes.ForEach(i => {
                if (x.nodes.IndexOf(i) == x.nodes.Count - 2) return;
                x.GetSegmentFromStartNode(i).Lots.ForEach(j => {
                j.Address =number + " " + x.roadName;
                number = number + 1;
                });
            });
        });
    }

    [InspectorButton("PathTest")]
    public bool pathTest;

    public void PathTest() {
        ReturnPath(nodes[13], nodes[29]);
    }

    public List<Node> ReturnPath(Node start, Node goal) {
        NodeGraph graph = new NodeGraph();
        var search = new DjNodeSearch(graph, start, goal);
        Node current = goal;
        List<Node> path = new List<Node>();
        while (current != start) {
            path.Add(current);
            current = search.cameFrom[current];
        }
        path.Reverse();
        Debug.Log(path.Count);
        path.ForEach(x => Instantiate(Resources.Load("boundingPoint"),x.pos() + new Vector3(0,1,0), Quaternion.identity, transform));
        return path;
    }
}

public class NodeGraph {
    NodeMap map = NodeMap.instance;
    List<Node> nodes;
    public NodeGraph() {
        nodes = map.nodes;
    }
    public Dictionary<Node,List<Node>> edges = new Dictionary<Node,List<Node>>();

    public List<Node> Neighbors(Node node) {
        return node.adjNodes;
    }
}

public interface WeightedGraph<Node> {
    float Cost(Node a, Node b);
    IEnumerable<Node> Neighbors(Node id);
}



public class PriorityQueue {
    // I'm using an unsorted array for this example, but ideally this
    // would be a binary heap. There's an open issue for adding a binary
    // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
    //
    // Until then, find a binary heap class:
    // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    // * http://xfleury.github.io/graphsearch.html
    // * http://stackoverflow.com/questions/102398/priority-queue-in-net

    
    private List<NodeFloatTuple> elements = new List<NodeFloatTuple>();

    public int Count {
        get { return elements.Count; }
    }

    public void Enqueue(Node item, float priority) {
        elements.Add(new NodeFloatTuple(item,priority));
    }

    public Node Dequeue() {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++) {
            if (elements[i].f < elements[bestIndex].f) {
                bestIndex = i;
            }
        }

        Node bestItem = elements[bestIndex].n;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

public class DjNodeSearch {
    public Dictionary<Node, Node> cameFrom
        = new Dictionary<Node, Node>();
    public Dictionary<Node, float> costSoFar
        = new Dictionary<Node, float>();

    public DjNodeSearch(NodeGraph graph, Node start, Node goal){
        var frontier = new PriorityQueue();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0) {
            var current = frontier.Dequeue();

            if (current.Equals(goal)) {
                break;
            }

            foreach(var next in graph.Neighbors(current)) {
                float newCost = costSoFar[current] + 1; //graph.Cost(current, next);
                if(!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next, 1);
                    cameFrom[next] = current;
                }
            }
        }
    }
}