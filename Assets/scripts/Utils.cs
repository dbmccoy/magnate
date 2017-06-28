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

    public static void VectorList(List<Vector3> list, Vector3 item) {
        foreach (var v in list) {
            if(Vector3.Distance(item, v) < .1f) {
                return;
            }
        }
        list.Add(item);
    }

    public static void VectorList(List<Vector3> list, List<Vector3> item) {
        if (item.GetType() != list.GetType()) {
            throw new Exception("Utils.UniqueList parameters have mismatched types");
        }
        foreach (var i in item) {
            bool contains = false;
            foreach (var v in list) {
                if (Vector3.Distance(i,v) < .1f) contains = true;
            }
            if(!contains) list.Add(i);
        }
    }

    public static void UniqueList<T>(List<T> list, List<T> range) {
        foreach (var item in range) {
            if (!list.Contains(item)) {
                list.Add(item);
            }
        }
    }


    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static Vector3 ReturnMaximalVector(List<Vector3> vectors, Utils.Direction dir) {
        Vector3 maximalVector = Vector3.zero;
        bool init = true;
        foreach (var v in vectors) {
            if (init) {
                maximalVector = v;
                init = false;
                continue;
            }
            if(dir == Left && v.x < maximalVector.x) {
                maximalVector = v;
            }
            if(dir == Right && v.x > maximalVector.x) {
                maximalVector = v;
            }
            if(dir == Up && v.z > maximalVector.z) {
                maximalVector = v;
            }
            if(dir == Down && v.z < maximalVector.z) {
                maximalVector = v;
            }
        }
        return maximalVector;
    }

    public static ArrayList NodeFloatTuple(Node n, float f) {
        ArrayList thing = new ArrayList();
        thing.Add(n); thing.Add(f);
        return thing;
    }

    public static Vector3 AverageVectors(List<Vector3> vectors) {
        Vector3 vec = vectors[0];
        for (int i = 1; i < vectors.Count; i++) {
            vec = vec + vectors[i];
        }
        vec = vec / vectors.Count;
        return vec;
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

public class NodeFloatTuple {
    public Node n;
    public float f;
    public NodeFloatTuple(Node _n, float _f) {
        n = _n;
        f = _f;
    }
}
