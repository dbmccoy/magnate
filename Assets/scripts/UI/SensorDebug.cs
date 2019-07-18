using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SensorDebug : MonoBehaviour {
    public Person Agent;
    public AgentDebug AgentDebug;
    public Text text;

    // Start is called before the first frame update
    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public Sensor Sensor;

    public List<Button> Buttons = new List<Button>();

    public List<LotMap> LotMaps;

    public void Set(Sensor s, AgentDebug a) {
        text = GetComponentInChildren<Text>();
        Sensor = s;
        AgentDebug = a;
    }

    public void OnClick() {

        LotMaps = Sensor.GetLotMaps();

        for (int i = 0; i < LotMaps.Count; i++) {
            var b = Instantiate(Resources.Load<Button>("UI/DynamicButton"), transform.position, Quaternion.identity);

            b.GetComponentInChildren<Text>().text = LotMaps[i].Name;
            b.transform.name = LotMaps[i].Name;
            b.transform.SetParent(this.transform);
            var l = b.gameObject.AddComponent<LotMapDebug>();
            l.Set(LotMaps[i],this);
            
            Buttons.Add(b);

            if (i > 0) {
                b.GetComponent<RectTransform>().localPosition = Buttons[i - 1].GetComponent<RectTransform>().localPosition + new Vector3(0, -30, 0);
            }
        }

    }

    // Update is called once per frame
    void Update() {

    }
}
