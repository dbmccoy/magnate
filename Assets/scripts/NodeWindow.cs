using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeWindow : EditorWindow {

    [MenuItem("Window/My Window")]

    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(NodeWindow));
    }

    void OnGUI() {

        if (GUILayout.Button("InsertNode")) {
            Road road = Selection.activeTransform.GetComponentInParent<Road>();
            Segment segment = Selection.activeTransform.GetComponentInParent<SegmentCollider>().segment;
            Node node = Instantiate(NodeMap.instance.node_prefab);
            node.transform.position = (segment.start() + segment.end()) / 2f;
            node.transform.parent = road.transform;
            road.SplitSegment(segment,node);
            Selection.activeTransform = node.transform;
            Debug.Log("insert node");
        }
        //EditorGUILayout.LabelField("Status: ", status);
    }
}
