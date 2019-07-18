using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LotMapDebug : MonoBehaviour {
    public Person Agent;
    public Text text;

    // Start is called before the first frame update
    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public LotMap LotMap;
    AgentDebug agentDebug;


    public void Set(LotMap l, SensorDebug s) {
        text = GetComponentInChildren<Text>();
        LotMap = l;
        agentDebug = s.AgentDebug;
    }

    bool isClicked;

    public void OnClick() {

        isClicked = !isClicked;

        if (isClicked) {
            agentDebug.AddLotMap(LotMap);
        }
        else {
            agentDebug.RemoveLotMap(LotMap);
        }

        agentDebug.VisualizeAggregateLotMap();

        /*
        float max = LotMap.Vals.Max();
        float min = LotMap.Vals.Min();

        foreach (var p in LotMap) {
            //Debug.Log(LotMap.Name + " : " + p.lot.Address + " : " + p.val);

            float adj = (p.val / max) * 255f;

            p.lot.GetComponent<MeshRenderer>().material.color = new Color(0,adj/255f,0);
        }
        */
    }

    // Update is called once per frame
    void Update() {

    }
}