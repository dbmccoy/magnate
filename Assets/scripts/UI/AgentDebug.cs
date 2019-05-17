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

    public void Set(Person a) {
        text = GetComponentInChildren<Text>();
        Agent = a;

        text.text = a.Name;
    }

    public void OnClick() {

        Sensors = Agent.GetComponents<Sensor>().ToList();
        for (int i = 0; i < Sensors.Count; i++) {
            var b = Instantiate(Resources.Load<Button>("UI/DynamicButton"), transform.position, Quaternion.identity);

            b.GetComponentInChildren<Text>().text = Sensors[i].GetType().ToString();
            b.transform.name = Sensors[i].GetType().ToString();
            var s = b.gameObject.AddComponent<SensorDebug>();
            s.Set(Sensors[i]);
            b.transform.SetParent(this.transform);
            Buttons.Add(b);

            if (i > 0) {
                b.GetComponent<RectTransform>().localPosition = Buttons[i - 1].GetComponent<RectTransform>().localPosition + new Vector3(0, -30, 0);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
