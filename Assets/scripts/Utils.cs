using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public static void UniqueList<T>(List<T> list,T item) {
        if(item.GetType() != list.GetType()) {
            throw new Exception("Utils.UniqueList parameters have mismatched types");
        }
        if (!list.Contains(item)) {
            list.Add(item);
        }
    }

    public static void UniqueList<T>(List<T> list, List<T> range) {
        foreach (var item in range) {
            if (!list.Contains(item)) {
                list.Add(item);
            }
        }
    }

    public enum Direction {
        left,
        right,
        up,
        down
    }

    public static Direction Left = Direction.left;
    public static Direction Right = Direction.right;
    public static Direction Up = Direction.up;
    public static Direction Down = Direction.down;
}
