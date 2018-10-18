using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using ReGoap.Core;
using ReGoap.Unity;
using ReGoap.Planner;

[RequireComponent(typeof(PlannerManager))]
public class GameManager : MonoBehaviour
{

    public Entity city;
    public Entity player;

    public ReGoapPlannerManager<string, object> Planner;

    public List<Entity> Entities = new List<Entity>();
    public List<Person> People = new List<Person>();

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
        Planner = GetComponent<PlannerManager>();

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
    }

    // Update is called once per frame
    public float timeSinceLastTick;

    public float TimeStep;

    void Update()
    {
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

        timeSinceLastTick += GameManager.i.TimeStep;

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

    private static GameManager _i;
    public static GameManager i
    {
        get
        {
            if (_i == null)
            {
                _i = GameObject.Find("GameManager").GetComponent<GameManager>();
            }

            return _i;
        }
    }
}
