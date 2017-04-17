using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Vector2[] V2dArray(List<Vector3> inputs) {
        List<Vector2> list = new List<Vector2>();
        foreach (var vector3 in inputs) {
            list.Add(new Vector2(vector3.x, vector3.z));
        }
        return list.ToArray();
    }

    public static List<Vector2> V2d(List<Vector3> inputs) {
        List<Vector2> list = new List<Vector2>();
        foreach (var vector3 in inputs) {
            list.Add(new Vector2(vector3.x, vector3.z));
        }
        return list;
    }


    public static Vector2 V2d(Vector3 input) {
        return new Vector2(input.x,input.z);
    }
}
