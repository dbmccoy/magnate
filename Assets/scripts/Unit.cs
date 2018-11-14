using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    public Building Building;
    public string Address;
    public int UnitNumber;
    public int SquareFeet;
    public int Bedrooms;

    public Unit(Building building, int number, int sqft, int bedrooms = 0)
    {
        Building = building;
        UnitNumber = number;
        SquareFeet = sqft;
        Bedrooms = bedrooms;
    }
}
