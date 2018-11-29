using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RentalBullitin
{
    public RentalBullitinEvent AddRentalToBullitinEvent;
    public List<UnitListing> Available = new List<UnitListing>();

    public void Add(UnitListing listing)
    {
        Debug.Log("add listing");
        Available.Add(listing);
        AddRentalToBullitinEvent.Invoke(listing);
    }

    public void AddListener(UnityAction<UnitListing> method) {
        AddRentalToBullitinEvent.AddListener(method);
    }

    public void Remove(UnitListing listing)
    {
        Available.Remove(listing);
    }

    private static RentalBullitin instance;

    private RentalBullitin() { }

    public static RentalBullitin Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new RentalBullitin();
            }

            return instance;
        }
    }
}

public class UnitListing : TemporalBase
{
    public Unit Unit;
    public Person PostedBy;
    public Entity OwnedBy;
    public float Price;
    public int Age;

    public UnitListing(Unit unit, Person person, float price)
    {
        Unit = unit;
        PostedBy = person;
        Price = price;
    }

    public void Apply(Person p)
    {
        PostedBy.GetComponent<RentOutUnitAction>().AddApplicant(p);
    }

    public void WithdrawApplication(Person p)
    {
        PostedBy.GetComponent<RentOutUnitAction>().RemoveApplicant(p);
    }

    public override void DayTick()
    {
        Age += 1;
    }
}

public class RentalBullitinEvent : UnityEvent<UnitListing> {

}
