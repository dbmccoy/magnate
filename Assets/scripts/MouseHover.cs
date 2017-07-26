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
    public ContextMenu contextMenu;
    public Navigator nav; //TEMPORARY FOR PRESENTATION
    public bool mouseDown;
    public GameObject X;
    public Node StartNode;

	// Use this for initialization
	void Start () {
		
	}

    public GameObject dm_Instantiate(GameObject obj, Vector3 pos, Quaternion rot, Transform t = null)
    {
        return Instantiate(obj, pos, rot, t);
    }

    Node selectedNode;


    // Update is called once per frame
    List<Node> path = new List<Node>();

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

                //if (hit.transform.tag == "Lot") contextInfo.
                //PrintLot(hit.transform.GetComponent<Lot>());
                Node goal = null;
                if (hit.transform.tag == "Node") {
                    if (goal == null || hit.transform != goal.transform)
                    { 
                        goal = hit.transform.GetComponent<Node>();
                        X.transform.position = hit.transform.GetComponent<Node>().pos();
                        if (path.Count > 0) path.ForEach(x => x.CameFromObj.GetComponentInChildren<SpriteRenderer>().material.color = Color.white);
                        path = StartNode.CachePath(hit.transform.GetComponent<Node>());
                        path.ForEach(x => x.CameFromObj.GetComponentInChildren<SpriteRenderer>().material.color = Color.blue);
                        
                        contextInfo.SetText(NodeMap.instance.ReturnCost(StartNode, goal).ToString());
                        Debug.Log(NodeMap.instance.ReturnCost(StartNode, goal));
                    }

                    if (Input.GetMouseButtonDown(0)) selectedNode = hit.transform.GetComponent<Node>();
                    if(selectedNode && hit.transform.GetComponent<Node>() != selectedNode && (hit.transform.GetComponent<Node>() != goal) )
                    {
                        
                        if (nav.GoalNode != null && nav.GoalNode == goal) return;
                        //List<Node> path = selectedNode.GetPathTo(goal);
                        nav.CurrentNode = selectedNode;
                        nav.SetGoal(goal);
                        nav.transform.position = selectedNode.pos();
                    }
                    //contextInfo.
                     //PrintNode(hit.transform.GetComponent<Node>());
                 }
                if (Input.GetMouseButtonUp(0))
                {
                    contextMenu.Open(hit.transform);
                }

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
