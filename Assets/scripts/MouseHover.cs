using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour {

    private static MouseHover _i;
    public static MouseHover i {
        get {
            if(_i == null) {
                _i = Camera.main.GetComponent<MouseHover>();
            }
            return _i;
        }
    }
    public Vector3 mousePos;
    public Transform hover;
    public GameObject ind;
    public ContextInfo contextInfo;
    public bool mouseDown;
    

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate() {
        //mousePos = Input.mousePosition;
        //mousePos.z = 1000f;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin + (Vector3.up * 10), ray.direction, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0)) {
            mouseDown = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            mouseDown = false;
        }

        if (mouseDown && hover) {
            hover.GetComponent<Entity>().UnHighlight();
        }


        foreach (var h in hits) {

            ind.transform.position = mousePos;
            mousePos = h.point;
            

            if (h.transform.GetComponent<Entity>()) {
                hit = h;
                if (hover == null) hover = hit.transform;
                if (hit.transform != hover) {
                    //hover.GetComponent<Entity>().UnHighlight();
                    hover = hit.transform;
                    //hover.transform.GetComponent<Entity>().Highlight();
                }
                contextInfo.panel.SetActive(true);
                contextInfo.Position(hit.point + new Vector3(0, 0, 5));

                if (hit.transform.tag == "Lot") contextInfo.
                    PrintLot(hit.transform.GetComponent<Lot>());

                if (hit.transform.tag == "Node") contextInfo.
                     PrintNode(hit.transform.GetComponent<Node>());

                break;
            }

            else {
                contextInfo.panel.SetActive(false);
                if (hover) {
                    //hover.GetComponent<Entity>().UnHighlight();
                    hover = null;
                }
            }
        }

	}
}
