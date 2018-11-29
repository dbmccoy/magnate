using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerObj : MonoBehaviour {

    Text text;
    Image image;

    public bool countUp;
    public bool running;
    public bool repeat;

    public float startingValue;
    public float rollOverValue;
    public float curValue;

    public float Speed = 1;
    public int Offset;

	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<Text>();
        image = transform.GetChild(0).GetComponent<Image>();
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;

        Set(startingValue);
	}

    public void Set(float v)
    {
        running = true;
        startingValue = v;
        curValue = v;
        text.text = v.ToString();
    }

    public void Complete()
    {
        if (repeat)
        {
            var carryOver = curValue - rollOverValue;
            curValue = startingValue + carryOver;
        }
        else
        {
            running = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.isPaused || !running)
        {
            return;
        }

        if (countUp)
        {
            curValue += GameManager.Instance.TimeStep * Speed;
        }
        else
        {
            curValue -= GameManager.Instance.TimeStep * Speed;
        }

        text.text = Mathf.FloorToInt(curValue + Offset).ToString();
        image.fillAmount = ((curValue) / rollOverValue);

        if(countUp && curValue >= rollOverValue)
        {
            Complete();
        }
        if(!countUp && curValue <= rollOverValue)
        {
            Complete();
        }

    }
}
