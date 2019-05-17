using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SensorDebug : MonoBehaviour {
    public Person Agent;
    public Text text;

    // Start is called before the first frame update
    void Awake() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public Sensor Sensor;

    public List<Button> Buttons = new List<Button>();

    public List<LotMap> LotMaps;

    public LotMap AggregateLotMap;
    private List<LotMap> currentLotMaps = new List<LotMap>();

    public void AddLotMap(LotMap m) {
        currentLotMaps.Add(m);
    }

    public  void RemoveLotMap(LotMap m) {
        currentLotMaps.Remove(m);
    }

    public void VisualizeAggregateLotMap() {

        for (int i = 0; i < currentLotMaps.Count; i++) {
            if(i == 0) {
                AggregateLotMap = currentLotMaps[i];
            }
            else {
                AggregateLotMap = AggregateLotMap + currentLotMaps[i];
            }
        }

        float max = AggregateLotMap.Vals.Max();
        float min = AggregateLotMap.Vals.Min();
        float avg = AggregateLotMap.Vals.Average();

        foreach (var p in AggregateLotMap) {
            //Debug.Log(LotMap.Name + " : " + p.lot.Address + " : " + p.val);

            float adj = ((p.val - avg) * 2f) /  (max - min);

            if(adj > 0) {
                p.lot.GetComponent<MeshRenderer>().material.color = new Color(0, adj, 0);
            }
            else {
                p.lot.GetComponent<MeshRenderer>().material.color = new Color(-adj,0, 0);
            }
        }
    }

    public void Set(Sensor s) {
        text = GetComponentInChildren<Text>();
        Sensor = s;

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
