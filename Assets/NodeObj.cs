using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class NodeObj : MonoBehaviour {
    private MeshRenderer mr;
	// Use this for initialization
	void Start () {
        mr = GetComponent<MeshRenderer>();
	}

    private void OnMouseOver() {
        mr.enabled = true;
    }
    private void OnMouseExit() {
        mr.enabled = false;
    }
}
