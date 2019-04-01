using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : IAsset{

    public Building Building;
    public string Address;
    public int UnitNumber;
    public int SquareFeet;
    public int Bedrooms;

    public static int Count;

    public Unit(int sqft, int bedrooms = 0)
    {
        SquareFeet = sqft;
        Bedrooms = bedrooms;
        Address = "Unit " + Count;
        Count += 1;
    }
    //IAsset implementation
    public string Name { get; set; }
    public Entity OwningEntity { get; set; }
    public string Class { get; set; }
    public float LastSalePrice { get; set; }
    public float ValueToOwner { get; set; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public void GrantActionsTo(Person p) {
        if (!p.GetComponent<RentOutUnitAction>()) {
            p.AddComponent<RentOutUnitAction>();
        }
        if (!p.GetComponent<RealEstateSensor>()) {
            p.AddComponent<RealEstateSensor>();
        }
    }

    public void RevokeActionsFrom(Person p) {

    }

    public float GetValue()
    {
        throw new System.NotImplementedException();
    }

    public void Transfer(Entity to)
    {
        throw new System.NotImplementedException();
    }

    public float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }
}
