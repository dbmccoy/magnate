using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AgentDebugToggle : MonoBehaviour

{
    public Button DynamicButton;

    public List<Button> Buttons;

    List<Person> Agents = new List<Person>();

    // Start is called before the first frame update
    public void OnClick() {
        Agents = GameManager.Instance.People;

        Agents.OrderByDescending(x => x.GetComponents<Sensor>().Length);

        for (int i = 0; i < Agents.Count; i++) {
            var b = Instantiate(DynamicButton, transform.position, Quaternion.identity, transform);
            Buttons.Add(b);
            b.gameObject.AddComponent<AgentDebug>();
            b.GetComponent<AgentDebug>().Set(Agents[i]);
            b.onClick.AddListener(Hide);

            if(i > 0) {
                b.GetComponent<RectTransform>().localPosition = Buttons[i - 1].GetComponent<RectTransform>().localPosition + new Vector3(0, -30, 0);
            }
        }

    }

    public void Hide() {
        Buttons.ForEach(x => x.GetComponent<AgentDebug>().text.enabled = false);
        Buttons.ForEach(x => x.image.enabled = false);

    }

    public void UnHide() {
        Buttons.ForEach(x => x.GetComponent<AgentDebug>().text.enabled = true);

        Buttons.ForEach(x => x.image.enabled = true);
    }
}
