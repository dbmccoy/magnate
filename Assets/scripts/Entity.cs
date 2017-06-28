using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    Color col;
    MeshRenderer mr;

    private void Start() {
       // col = GetComponent<MeshRenderer>().material.color;
    }
    

	// Use this for initialization
	public void Highlight () {
        if (col != Color.green) col = GetComponent<MeshRenderer>().material.color;
        mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.green;
	}

    public void UnHighlight() {
        if(mr)mr.material.color = col;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
