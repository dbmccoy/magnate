using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : IOwnable{

    public Building Building;
    public string Address;
    public int UnitNumber;
    public int SquareFeet;
    public int Bedrooms;

    public Unit(int sqft, int bedrooms = 0)
    {
        SquareFeet = sqft;
        Bedrooms = bedrooms;
    }

    public string Name { get; set; }
    public Entity OwningEntity { get; set; }

    public float GetValue()
    {
        throw new System.NotImplementedException();
    }

    public void Transfer(Entity to)
    {
        throw new System.NotImplementedException();
    }
}
