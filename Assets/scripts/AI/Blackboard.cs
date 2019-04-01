using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard 
{
    private static Blackboard instance;

    public static Blackboard Instance {
        get {
            if(instance == null) {
                instance = new Blackboard();
            }

            return instance;
        }
    }

    public List<Person> People = new List<Person>();
    public List<Lot> Lots = new List<Lot>();
    public List<Building> Buildings = new List<Building>();
    public List<Neighborhood> Neighborhoods = new List<Neighborhood>();

    public float CityTaxValue;

    void UpdateCityTaxValue() {

    }

    public float CityIncome { get; set; }
    public float CityDebt { get; set; }
    public float ReserveInterestRate { get; set; }

    public float PropertyTaxRate { get; set; }

}
