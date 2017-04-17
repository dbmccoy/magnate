using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockMap : MonoBehaviour {

    public NodeMap nodeMap;
    public Block blockPrefab;
    public List<Block> blocks;
    List<Node> nodes;
    List<Segment> nodeSegments;
    public GameObject indicatorPrefab;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        nodes = nodeMap.nodes;
    }

    private static BlockMap _blockMap;
    public static BlockMap instance {
        get {
            if(_blockMap == null) {
                _blockMap = GameObject.Find("Blocks").GetComponent<BlockMap>();
            }
            return _blockMap;
        }
    }

    [InspectorButton("QueryBlockButton")]
    public bool update;
    public void QueryBlockButton() {
        for (int i = 0; i < 1; i++) {
            Vector3 _pos = new Vector3(0, 0, 0); //Random.insideUnitSphere * Random.Range(0f, 50f); _pos.y = 0;
            QueryBlockBounds(_pos);
        }

    }

    void QueryBlockBounds(Vector3 pos, int ray_count = 12, float distance = 50f) {
        nodeSegments = nodeMap.nodeSegments;
        Vector3 rayVec = new Vector3(0, 0, distance);
        List<Segment> boundingSegments = new List<Segment>();

        //cast rays in all directions to find the road segments that enclose the given point

        for (int j = 0; j < ray_count; j++) {
            Vector3 vec = Quaternion.Euler(0, (j / 12f) * 360f, 0) * rayVec;
            Debug.DrawLine(pos, pos + rayVec);
            RaycastHit hit;
            Ray ray = new Ray(pos, vec);
            if (Physics.Raycast(ray, out hit, distance) && hit.transform.tag == "nodeSegment") {
                //Instantiate(indicatorPrefab, hit.point, Quaternion.identity, transform); //instantiate hit indicator (debug)
                boundingSegments.Add(hit.transform.parent.GetComponent<SegmentCollider>().segment);
            }
        }
        if(boundingSegments.Count > 2) {
            CreateBlock(pos,boundingSegments);
        }
     }

    void CreateBlock(Vector3 _pos, List<Segment> segments) {
        Block newBlock = Instantiate(blockPrefab, transform);
        newBlock.transform.position = _pos;
        newBlock.InitBlock(segments);
        blocks.Add(newBlock);
        Debug.Log("instantiated new block at " + _pos);
    }

    Segment ReturnBoundingSegment(Vector3 origin, Vector3 hitPoint, Road hitRoad) {
        Segment hitSegment = hitRoad.segments[0];
        float distance = 0;
        foreach (Segment segment in hitRoad.segments) {
            Vector3 intersection;
            if(Math3d.LineLineIntersection(out intersection, origin, hitPoint, segment.start(), segment.end())) {
                if(Vector3.Distance(intersection,origin) > distance) {
                    hitSegment = segment;
                    distance = Vector3.Distance(intersection, origin);
                }
            }
        }
        return hitSegment;
    }
}
        //old segment to segment logic
            /*if(Math3d.AreLineSegmentsCrossing(newSegment.start, newSegment.end, nodeMap.nodeSegments[i].start, nodeMap.nodeSegments[i].end)) {
                if (!boundingSegments.Contains(nodeSegments[i])) {
                    Vector3 intersectionPoint;
                    Math3d.LineLineIntersection(out intersectionPoint, newSegment.start, newSegment.vector,
                                                nodeMap.nodeSegments[i].start, nodeMap.nodeSegments[i].vector);
                    Debug.DrawLine(newSegment.start, intersectionPoint, Color.blue);
                    boundingSegments.Add(nodeSegments[i]);

                }
                Debug.Log(nodeSegments[i].road.name);
                    
                break;              
            }*/
        
        //instantiate a new block using the segments 
        //if (boundingSegments.Count >= 3) {

        //}
    


