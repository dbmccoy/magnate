using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Road))]
public class ObjectBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Road myScript = (Road)target;
        if (GUILayout.Button("Add Node")) {
            myScript.AddNode();
        }
    }
}

