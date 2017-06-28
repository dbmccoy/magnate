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
        Vector3[] positions = {
            new Vector3(0, 0, 0), new Vector3(10, 0, 0), new Vector3(-10, 0, 0), new Vector3(0, 0, 10),
            new Vector3(0, 0, -10), new Vector3(10, 0, 10), new Vector3(10, 0, -10), new Vector3(-10, 0, 10),
            new Vector3(-10, 0, -10),
        };
        for (int i = 0; i < positions.Length; i++) {
            Vector3 _pos = positions[i]; //Random.insideUnitSphere * Random.Range(0f, 50f); _pos.y = 0;
            QueryBlockBounds(_pos);
        }
        //ORDER LOTS WITHIN EACH SEGMENT AND ADD ADDRESSES
        nodeMap.roads.ToList().ForEach(x => {
            x.nodes = x.OrderNodes();
            x.segments.ForEach(i => i.OrderedLots());
        });
        nodeMap.roads.ToList().ForEach(x => x.AssignLotAddresses());
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
        newBlock.InitBlock(segments);
        blocks.Add(newBlock);
        newBlock.BlockRayButton();

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
    


