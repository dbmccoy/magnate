using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug {

    public static void Mark(Vector3 v)
    {
        Debug.Log("call");
        DebugController.i.Mark(v);
    }
}

