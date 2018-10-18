using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    private static ZoneManager _i;
    public static ZoneManager i
    {
        get
        {
            if(_i == null)
            {
                _i = Camera.main.GetComponent<ZoneManager>();
            }
            return _i;
        }
    }

    public Zone _CS;
    private Zone _R5;

    public Zone CS
    {
        get
        {
            if (_CS == null) AddZones();
            return _CS;
        }
        set
        {
            _CS = value;
        }
    }

    public Zone R5
    {
        get
        {
            if (_R5 == null) AddZones();
            return _R5;
        }
        set
        {
            _R5 = value;
        }
    }

    // Use this for initialization

    private void Awake()
    {
        AddZones();
    }


    [InspectorButton("AddZones")]
    public bool AddZone;

    void AddZones () {
        _CS = new Zone(Zone.ZoneClass.CS, 30, 0, 0, 5, 0);
        _R5 = new Zone(Zone.ZoneClass.R5, 30, 10, 5, 20, 2500);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[SerializeField]
public class Zone
{
    public string name;
    public ZoneClass zone;
    public float maxHeight;
    public float minSetBackFront;
    public float minSetBackSide;
    public float minSetBackRear;
    public int minOutdoorArea;

    public Zone(ZoneClass _class,float height, float minFront, float minSide, float minRear, int minOutdoor)
    {
        zone = _class;
        maxHeight = height;
        minSetBackFront = minFront;
        minSetBackSide = minSide;
        minSetBackRear = minRear;
        minOutdoorArea = minOutdoor;
    }

    public enum ZoneClass
    {
        R2_5,
        R5,
        R7_5,
        CS
    }
}
