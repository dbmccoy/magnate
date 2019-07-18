using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour {

    private Color col;
    private Material mat;

    public MeshFilter Filter { get; private set; }
    public MeshRenderer Render { get; private set; }

    private bool isHighlight;
    private bool wasHighlightThisFrame;

    public virtual void Start()
    {
        Filter = GetComponent<MeshFilter>();
        Render = GetComponent<MeshRenderer>();
    }

    public void LateUpdate() {
        if (isHighlight && wasHighlightThisFrame == false) {
            UnHighlight(); 
        }
        wasHighlightThisFrame = false;
    }

    // Use this for initialization
    public virtual void Highlight () {
        wasHighlightThisFrame = true;
        if (!isHighlight) {
            Render = GetComponent<MeshRenderer>();
            col = Render.material.color;
            Render.material.color = Color.green;
            isHighlight = true;
        }
    }

    public virtual void UnHighlight() {
        isHighlight = false;
        Render.material.color = col;
    }
}
