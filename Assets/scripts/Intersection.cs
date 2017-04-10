using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour{

    public List<Segment> segments = new List<Segment>();

    public void Init(List<Segment> _segments) {
        segments = _segments;
    }
}
