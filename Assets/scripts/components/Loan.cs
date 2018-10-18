using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loan : IOwnable, ITemporal {

    public string Name { get; set; }
    public Entity OwningEntity { get; set; }

    public Entity Borrower;

    public IOwnable Collateral;

    public float Balance;
    public float Rate;
    public int Term;
    public int PeriodsDelinquent;
    public float TotalPayments;

    public Loan(Bank bank, Entity borrower, IOwnable collateral,
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

    public float GetValue()
    {
        return Balance; //not true
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
