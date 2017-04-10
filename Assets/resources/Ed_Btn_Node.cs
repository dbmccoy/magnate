﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class Ed_Btn_Node : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Node myScript = (Node)target;
        if (GUILayout.Button("Add Node")) {
            myScript.AddNode();
        }
        if (GUILayout.Button("Remove Node")) {
            myScript.RemoveNode();
        }
    }
}