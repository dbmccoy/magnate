using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Entity city;
    public Entity player;

    public List<Entity> Entities = new List<Entity>();
    public List<Person> People = new List<Person>();

    public float BaseLandCost;

    public bool isPaused;

    public TimerObj DayTimer;
    public TimerObj MonthTimer;

    public int GameSpeed;
    public int Day;
    public int Month;
    public int Year;

    public float MonthProgress;
    public float YearProgress;
    public int DaysSinceStart;

    public UnityEvent DayTickEvent;
    public UnityEvent MonthTickEvent;

    // Use this for initialization
    void Awake()
    {
        DayTickEvent = new UnityEvent();
        MonthTickEvent = new UnityEvent();

        DayTimer.countUp = true;
        DayTimer.startingValue = 0;
        DayTimer.Offset = 1;
        DayTimer.rollOverValue = 30;
        DayTimer.repeat = true;

        MonthTimer.countUp = true;
        MonthTimer.startingValue = 0;
        MonthTimer.Offset = 1;
        MonthTimer.rollOverValue = 12;
        MonthTimer.Speed = (1f / 30f);
        MonthTimer.repeat = true;

        Day = 1;
        Month = 1;
        Year = 1980;

        GoalDebug = GameObject.Find("GoalDebug").GetComponent<Text>();
        BullitinDebug = GameObject.Find("BullitinDebug").GetComponent<Text>();
    }

    public float GetTimeStamp() {
        return DaysSinceStart;
    }

    // Update is called once per frame
    public float timeSinceLastTick;

    public float TimeStep;

    Text GoalDebug;
    Text BullitinDebug;

    void Update()
    {
        GoalDebug.text = "";
        //switch to UI manager or something
        foreach (var person in People) {
            try {
                var deletethis = person.name != null;
            }
            catch {
                People.Remove(person);
            }
            var t = "";

            foreach (var item in person.GoalQueue) {
                t = t + "    " + GoapAgent.prettyPrint(item) + "\n";
            }

            if(t == "") {
                t = "Idle";
            }

            GoalDebug.text = GoalDebug.text +"\n"+ person.name + ": \n   <color=blue>" + person.GetPlan() + "</color>" + "\n" + t;

        }
        /*
        BullitinDebug.text = "";

        foreach (var item in RentalBullitin.Instance.Available) {
            BullitinDebug.text = BullitinDebug.text + item.Unit.Address + "\n";
        }
        */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
        }

        if (!isPaused)
        {
            TimeStep = Time.deltaTime * GameSpeed;
        }
        else
        {
            TimeStep = 0f;
        }

        timeSinceLastTick += GameManager.Instance.TimeStep;

        if (timeSinceLastTick >= 1)
        {
            timeSinceLastTick = timeSinceLastTick - 1;
            DayTick();
        }
    }


    private void LateUpdate()
    {
        
    }

    public void DayTick()
    {
        DayTickEvent.Invoke();

        Day++;
        DaysSinceStart++;
        if (Day == 31)
            MonthTick();
    }

    public void MonthTick()
    {
        MonthTickEvent.Invoke();

        Day = 1;
        Month++;
        if (Month == 13)
            YearTick();
    }

    public void YearTick()
    {
        Month = 1;
        Year++;
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            }

            return instance;
        }
    }
}

