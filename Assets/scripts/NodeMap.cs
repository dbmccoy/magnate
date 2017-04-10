using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeMap : MonoBehaviour {

    public Road[] roads;
    public List<Segment> nodeSegments;
    public List<Intersection> intersections;
    public List<string> possibleStreetNames;
    public List<string> currentStreetNames;
    //public List<Node> allNodes;

	// Use this for initialization
	void Start () {
	}

    public Intersection intersectionPrefab;
    List<Vector3> UpdatedIntersectionPositions;

    public void AddIntersection(Vector3 pos, Road road1, Road road2, Segment seg1 = null, Segment seg2 = null) {
        UpdatedIntersectionPositions.Add(pos);
        if (CheckForIntersection(pos)) return;
        Intersection intersection = Instantiate(intersectionPrefab, transform);
        intersection.transform.position = pos;
        intersection.transform.name = road1.roadName + " and " + road2.roadName;
        intersections.Add(intersection);
        if(!road1.intersections.Contains(intersection)) road1.intersections.Add(intersection);
        if(!road2.intersections.Contains(intersection)) road2.intersections.Add(intersection);
        if(seg1 != null) seg1.AddIntersection(intersection);
        if(seg2 != null) seg2.AddIntersection(intersection);
        intersection.Init(new List<Segment> { seg1, seg2 });
    }

    public void RemoveIntersection(Intersection _intersection) {
        intersections.Remove(_intersection);
        DestroyImmediate(_intersection.gameObject);
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
        Selection.activeObject = newRoad;
        RoadName(newRoad.GetComponent<Road>());
    }

    Road ReturnRoad() {
        GameObject newRoad = Instantiate(Resources.Load("road"), transform) as GameObject;
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
        List<Intersection> intersectionsToRemove = new List<Intersection>();
        foreach (var item in intersections) {
            if (!UpdatedIntersectionPositions.Contains(item.transform.position)) {
                intersectionsToRemove.Add(item);
            }
        }
        foreach (var item in intersectionsToRemove) {
            RemoveIntersection(item);
        }
        intersectionsToRemove.Clear();
        UpdatedIntersectionPositions.Clear();
    }

    bool CheckForIntersection(Vector3 pos) {
        if (intersections.Count == 0) return false;
        foreach (var item in intersections) {
            if(item.transform.position == pos) {
               // Debug.Log("intersection already exists");
                return true;
            }
        }
        //Debug.Log("intersection position open");
        return false;
    }
	
	// Update is called once per frame
	public void PopulateNodeMap () {
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
                                    Debug.DrawLine(roads[i].segments[j].start(),roads[i].segments[j].end(), Color.red);
                                }
                                road2start = roads[k].segments[t].start();
                                road2end = roads[k].segments[t].end();
                                road2vec = roads[k].segments[t].vector();
                               
                                if (Math3d.AreLineSegmentsCrossing(road1start, road1end, road2start, road2end)) {
                                    Vector3 intersection;
                                    Math3d.LineLineIntersection(out intersection, road1start, road1vec, road2start, road2vec);

                                    AddIntersection(intersection, roads[i], roads[k],roads[i].segments[j],roads[k].segments[t]);

                                }
                            }
                        }
                    }
                }
            }
        }
        Debug.Log(nodeSegments.Count + " road nodes in NodeMap");
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
}
