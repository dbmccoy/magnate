using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Loan : IAsset, ITemporal {

    public string Name { get; set; }
    public Entity OwningEntity { get; set; }

    public Entity Borrower;
    public List<string> Tags { get; set; }

    public IAsset Collateral;

    public float Balance;
    public float Rate;
    public int Term;
    public int PeriodsDelinquent;
    public float TotalPayments;

    public Loan(Bank bank, Entity borrower, IAsset collateral,
                float amount, float rate, int term)
    {
        OwningEntity = bank;
        Borrower = borrower;
        Collateral = collateral;
        Balance = amount;
        Rate = rate;
        Term = term;
        AddTemporal();
    }

    public string Class { get; set; }
    public float LastSalePrice { get; set; }
    public float ValueToOwner { get; set; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public void GrantActionsTo(Person p) {

    }

    public void RevokeActionsFrom(Person p) {

    }

    public float GetValue()
    {
        return Balance; //not true
    }

    public float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }

    public float ComputeInterest()
    {
        return Balance * (Rate / 12f);
    }

    public void ReceivePayment(float amount)
    {
        OwningEntity.Account.Credit(amount);
        Balance -= amount;
        TotalPayments += amount;
        //Debug.Log("received payment of " + amount + ", balance is " + Balance + ", lifetime payments are " + TotalPayments);
    }

    public void Transfer(Entity to)
    {

    }

    public void DayTick()
    {

    }

    public void MonthTick()
    {
        Balance = Balance + ComputeInterest();
    }

    public void AddTemporal()
    {
        var temporal = new Temporal(this);
    }
}
