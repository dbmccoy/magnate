﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseDrag : MonoBehaviour {

    private static MouseDrag _instance;

    public static MouseDrag Instance {
        get {
            if(_instance == null) {
                _instance = Camera.main.GetComponent<MouseDrag>();
            }

            return _instance;
        }
    }

    public Vector3 LastMousePosition;
    public Vector3 dragToPoint;
    public Vector3 dragVector;
    public float OffsetY;
    Vector3 last;
    bool isDragging;
    bool canZoom = true;

    public void CanZoom(bool b) {
        canZoom = b;
    }

	// Use this for initialization
	void Start () {

    }

    Vector3 camOrigPos;
    Ray mouseRay;
    float rayLength;

    // Update is called once per frame
    void Update () {


        if (Input.GetMouseButtonDown(1)) {
            //camOrigPos = Camera.main.transform.position;
            LastMousePosition = MouseHover.i.mousePos;
            isDragging = true;
            //grabPoint.y = OffsetY;
        }
        if (isDragging) {
            dragVector = (LastMousePosition - MouseHover.i.mousePos);
            dragVector.y = 0;

            Camera.main.transform.Translate(dragVector, Space.World);

            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            rayLength = (mouseRay.origin.y / mouseRay.direction.y);
            LastMousePosition = mouseRay.origin - (mouseRay.direction * rayLength);
            
            //grabPoint = Camera.main.transform.position;
        }

        //zoom camera w scrollwheel
        float scrollAmount = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -.1f, .1f);
        if(Mathf.Abs(scrollAmount) > .01f)
        {
            Vector3 dir = Camera.main.transform.position - MouseHover.i.mousePos;
            if(Camera.main.transform.position.y > 200f) scrollAmount = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -.1f, 0f);
            if(Camera.main.transform.position.y < 10) scrollAmount = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), 0f, .1f);

            if (canZoom) {
                Camera.main.transform.Translate(dir * scrollAmount, Space.World);
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }
	}
}
