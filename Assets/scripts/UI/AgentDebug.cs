using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AgentDebug : MonoBehaviour
{
    public Person Agent;
    public Text text;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public AgentDebugToggle Parent;

    public List<Button> Buttons = new List<Button>();

    public List<Sensor> Sensors;
    public Text stateText;

    public void Set(Person a) {
        text = GetComponentInChildren<Text>();
        Agent = a;

        text.text = a.Name;
    }

    public void OnClick() {
        //SetContextSensors();
        SetContextState();

    }

    public enum State {
        None,
        AgentState,
        Sensors
    }

    public State currentState;

    void SetContextState() {
        var b = Instantiate(Resources.Load<Button>("UI/DynamicButton"), transform.position, Quaternion.identity);

        var state = Agent.GetComponent<GoapAgent>().dataProvider.getWorldState();

        var s = "";

        foreach (var item in state) {
            s += item.Key + " > " + item.Value +"\n";
        }

        b.GetComponentInChildren<Text>().text = s;

        b.transform.SetParent(this.transform);
        Buttons.Add(b);
    }

    void SetContextSensors() {
        Sensors = Agent.GetComponents<Sensor>().ToList();
        for (int i = 0; i < Sensors.Count; i++) {
            var b = Instantiate(Resources.Load<Button>("UI/DynamicButton"), transform.position, Quaternion.identity);

            b.GetComponentInChildren<Text>().text = Sensors[i].GetType().ToString();
            b.transform.name = Sensors[i].GetType().ToString();
            var s = b.gameObject.AddComponent<SensorDebug>();
            s.Set(Sensors[i], this);
            b.transform.SetParent(this.transform);
            Buttons.Add(b);

            if (i > 0) {
                b.GetComponent<RectTransform>().localPosition = Buttons[i - 1].GetComponent<RectTransform>().localPosition + new Vector3(0, -30, 0);
            }
        }
    }

    public LotMap AggregateLotMap;
    private List<LotMap> currentLotMaps = new List<LotMap>();

    public void AddLotMap(LotMap m) {
        currentLotMaps.Add(m);
    }

    public void RemoveLotMap(LotMap m) {
        currentLotMaps.Remove(m);
    }

    public void VisualizeAggregateLotMap() {

        for (int i = 0; i < currentLotMaps.Count; i++) {
            if (i == 0) {
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

            float adj = ((p.val - avg) * 2f) / (max - min);

            if (adj > 0) {
                p.lot.GetComponent<MeshRenderer>().material.color = new Color(0, adj, 0);
            }
            else {
                p.lot.GetComponent<MeshRenderer>().material.color = new Color(-adj, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == State.AgentState) {
            SetContextState();
        }
        if(currentState == State.Sensors) {
            SetContextSensors();
        }
    }
}
