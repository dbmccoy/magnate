using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlannerDebug : MonoBehaviour
{
    public Text text;

    public HashSet<KeyValuePair<string, object>> worldState;
    public HashSet<KeyValuePair<string, object>> goal;

    public string state;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var s = "Goal: \n";

        foreach (var item in goal) {
            s += item.Key + " > " + item.Value + "\n";
        }

        s += "WorldState: \n";

        foreach (var item in worldState) {
            s += item.Key + " > " + item.Value + "\n";
        }

        text.text = s;
    }
}
