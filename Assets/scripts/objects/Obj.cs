using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour {

    private Color col;
    public MeshFilter Filter { get; private set; }
    public MeshRenderer Render { get; private set; }

    public virtual void Start()
    {
        Filter = GetComponent<MeshFilter>();
        Render = GetComponent<MeshRenderer>();
    }

	// Use this for initialization
	public virtual void Highlight () {
        if (col != Color.green) col = GetComponent<MeshRenderer>().material.color;
        Render = GetComponent<MeshRenderer>();
        Render.material.color = Color.green;
	}

    public virtual void UnHighlight() {
        if(Render)Render.material.color = col;
    }
}
