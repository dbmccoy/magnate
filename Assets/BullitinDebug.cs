using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BullitinDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public Text Contents;
    public List<GameObject> Buttons;
    public GameObject ReturnButton;

    string contents;

    public void DisplayBullitin(string s) {
        contents = "";
        switch (s) {
            case "AssetBullitin":
                contents += "AssetBullitin\n";
                AssetBullitin.Instance.Available.ForEach(x => contents += x.Asset.Name + "\n");
                break;
            case "RentalBullitin":
                contents += "RentalBulltin\n";
                RentalBullitin.Instance.Available.ForEach(x => contents += x.Unit.Name + "\n");
                break;
            default:
                contents = "empty";
                break;
        }

        Buttons.ForEach(x => x.SetActive(false));
        ReturnButton.SetActive(true);

        Contents.text = contents;
    }

    public void Return() {
        ReturnButton.SetActive(false);
        Buttons.ForEach(x => x.SetActive(true));
        Contents.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
