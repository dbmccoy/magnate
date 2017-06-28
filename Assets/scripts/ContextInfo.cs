using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextInfo : MonoBehaviour {

    public GameObject panel;
    public RectTransform rt;
    Text _text;

	// Use this for initialization
	void Start () {
        panel = this.gameObject;
        _text = panel.GetComponentInChildren<Text>();
        rt = panel.GetComponent<RectTransform>();
        Position(new Vector3(-5, 0, 5));
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PrintLot(Lot lot) {
        _text.text = lot.Address;
    }

    public void PrintNode(Node node) {
        _text.text = node.transform.name;
    }

    public void Position(Vector3 v) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(v);
        panel.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
    }
}
