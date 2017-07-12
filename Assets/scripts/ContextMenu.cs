using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour {


    public GameObject panel;
    public RectTransform rt;
    Text _text;
    Button _button;

    // Use this for initialization
    void Start()
    {
        panel = this.gameObject;
        _text = panel.GetComponentInChildren<Text>();
        _button = panel.GetComponentInChildren<Button>();
        rt = panel.GetComponent<RectTransform>();
        Position(new Vector3(-100000, 0, 0));
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Open(Transform entity)
    {
        if(entity.tag == "Lot")
        {
            OpenLot(entity.GetComponent<Lot>());
        }
    }

    public void OpenLot(Lot lot)
    {
        Position(lot.center + new Vector3(7, 0, 7));
        _button.onClick.AddListener(lot.Build);
    }

    public void Position(Vector3 v)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(v);
        panel.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
    }
}
