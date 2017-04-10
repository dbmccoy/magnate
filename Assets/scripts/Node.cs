using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 pos() {
        return transform.position;
    }

    public void AddNode() {
        transform.parent.GetComponent<Road>().AddNode();
    }

    public void RemoveNode() {
        transform.parent.GetComponent<Road>().RemoveNode(GetComponent<Node>());
    }
}
