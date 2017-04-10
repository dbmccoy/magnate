using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SegmentCollider : MonoBehaviour {

    public Segment segment;
    public Vector3 start;
    public Vector3 end;
    int segmentNumber;
    BoxCollider col;
    Road road;

	// Use this for initialization
	public void Init (Segment _segment, Road _road) {
        segment = _segment;
        //segmentNumber = num;
        road = _road;
        col = this.gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.GetComponent<BoxCollider>().isTrigger = true;
        col.transform.tag = "nodeSegment";
        start = segment.start();
        end = segment.end();
        UpdateCollider();
    }
	
	// Update is called once per frame
	void Update () {
        //UpdateCollider();
	}

    [InspectorButton("UpdateCollider")]
    public bool update;

    public void UpdateCollider() {
        //Debug.Log("update collider called");
        //segment = road.segmentToObj.FirstOrDefault(x => x.Value == GetComponent<SegmentCollider>()).Key; //reverse lookup on dictionary
        //segment = road.segments[segmentNumber];
        //Debug.Log(segment.road.name);
        start = segment.start();
        end = segment.end();

        float lineLength = Vector3.Distance(start, end); // length of line
        //Debug.Log(segment.road.roadName);
        col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (start + end) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        if(lineLength > .1f) {
            float angle = (Mathf.Abs(start.z - end.z) / Mathf.Abs(start.x - end.x));
            if ((start.z < end.z && start.x > end.x) || (end.z < start.z && end.x > start.x)) {
                angle *= -1;
            }
            angle = Mathf.Rad2Deg * Mathf.Atan(angle);
            col.transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
        //col.transform.Rotate(0, -angle, 0);
    }
}
