using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour {

    private static DebugController _i;
    public static DebugController i
    {
        get
        {
            if (_i == null)
            {
                _i = Camera.main.GetComponent<DebugController>();
            }
            return _i;
        }
    }

    public void Mark(Vector3 v)
    {
        Instantiate(Resources.Load("boundingPoint"), v, Quaternion.identity, transform);
        Debug.Log(v);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
