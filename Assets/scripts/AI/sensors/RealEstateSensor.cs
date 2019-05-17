using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RealEstateSensor : Sensor {

    public List<Building> myBuildings = new List<Building>();
    public List<Unit> myUnits = new List<Unit>();

    public List<Unit> listedUnits = new List<Unit>();
    public List<Unit> rentedUnits = new List<Unit>();

    public override List<LotMap> GetLotMaps() {
        return null;
    }

    public override void Sense()
    {
        myBuildings = new List<Building>();
        myUnits = new List<Unit>();


        foreach (var asset in person.Entity.Assets)
        {
            if (asset is Building b)
            {
                myBuildings.Add(b);
            }

            if (asset is Unit u)
            {
                myUnits.Add(u);
            }
        }

        // do thinking, decide to put all units on rental market
        foreach (var unit in myUnits)
        {
            if (!listedUnits.Has(unit) && !rentedUnits.Has(unit) )
            {
                //listedUnits.Add(unit);
                //Debug.Log(person.name + " rent out unit goal added " + unit.Address);
                person.AddGoal("rentOut"+unit.Address, unit);
            }
        }
    }

    public override HashSet<KeyValuePair<string, object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        return data;
    }

    public override void AddTemporal()
    {
        new Temporal(this);
    }

    public override void DayTick()
    {
        base.DayTick();
        Sense();
    }

    public override float EvaluateAsset(IAsset asset) {
        return 0f;
    }
}
