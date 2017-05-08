using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lot : MonoBehaviour {

    public List<Vector3> verts;
    public List<Vector3> points;
    public Vector3 center;
    public string horizontal;
    public LotInfo info;

	// Use this for initialization
	public void init(LotInfo _info) {
	    info = _info;
	    center = info.Center;
	    verts = info.LotVerts;
	    points = info.points;
	    foreach (var v in verts) {
	        Instantiate(info.ParentBlock.redMarker, v, Quaternion.identity, transform);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
