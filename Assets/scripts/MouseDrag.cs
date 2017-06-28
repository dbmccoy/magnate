using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour {

    public Vector3 LastMousePosition;
    public Vector3 dragToPoint;
    public Vector3 dragVector;
    public float OffsetY;
    Vector3 last;
    bool isDragging;

	// Use this for initialization
	void Start () {

    }

    Vector3 camOrigPos;
    Ray mouseRay;
    float rayLength;

    // Update is called once per frame
    void Update () {


        if (Input.GetMouseButtonDown(0)) {
            //camOrigPos = Camera.main.transform.position;
            LastMousePosition = MouseHover.i.mousePos;
            isDragging = true;
            //grabPoint.y = OffsetY;
        }
        if (isDragging) {
            dragVector = (LastMousePosition - MouseHover.i.mousePos);

            Camera.main.transform.Translate(dragVector, Space.World);

            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            rayLength = (mouseRay.origin.y / mouseRay.direction.y);
            LastMousePosition = mouseRay.origin - (mouseRay.direction * rayLength);
            
            //grabPoint = Camera.main.transform.position;
        }
        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }
	}
}
