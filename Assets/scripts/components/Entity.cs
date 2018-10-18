using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class Entity : IOwnable, ITemporal {

    public string Name { get; set; }
    public List<IOwnable> Assets = new List<IOwnable>();
    public List<IOwnable> Liabilities = new List<IOwnable>();
    public Account Account;

    public List<Contract> Contracts = new List<Contract>();
    public List<Project> Projects = new List<Project>();

    public List<WorkUnit> WorkUnits = new List<WorkUnit> { new WorkUnit()};

    public Entity OwningEntity { get; set; }

    public Entity(string name)
    {
        Name = name;
        AddTemporal();
    }

    public virtual float GetValue()
    {
        float value = 0f;
        Assets.ForEach(x => value += x.GetValue());
        Liabilities.ForEach(x => value -= x.GetValue());
        return value;
    }

    public virtual void AcquireAsset(IOwnable asset)
    {
        Assets.Add(asset);
        asset.OwningEntity = this;
    }

    protected virtual void DivestAsset(IOwnable asset)
    {
        Assets.Remove(asset);
    }

    public virtual void DischargeLiability(IOwnable liability)
    {
        Liabilities.Remove(liability);
    }

    public virtual void CheckContracts()
    {
        foreach (var contract in Contracts)
        {
            foreach (var term in contract.SelfTerms(this))
            {
                DoWork(term);
            }
        }
    }

    public virtual void DoWork(ContractTerm term)
    {

    }

    public void Transfer(Entity to)
    {

    }

    public void DayTick()
    {

    }

    public void MonthTick()
    {
        Liabilities.ForEach(x =>
        {
            if (x is Loan l)
            {
                l.ReceivePayment(Account.Debit(1000f));
            }
        });
    }

    public void AddTemporal()
    {
        var temporal = new Temporal(this);
    }
}



public class Temporal
{
    ITemporal Attached;
    public UnityAction DayTickListener { get; private set; }
    public UnityAction MonthTickListener { get; private set; }
    
    public Temporal(ITemporal attached)
    {
        Attached = attached;
        DayTickListener = new UnityAction(Attached.DayTick);
        MonthTickListener = new UnityAction(Attached.MonthTick);
        GameManager.i.DayTickEvent.AddListener(DayTickListener);
        GameManager.i.MonthTickEvent.AddListener(MonthTickListener);
    }
}



public class CashTransfer : ITransferable, ICondition
{
    public Entity From;
    public Entity To;

    public float Amount;
    public bool isComplete;

    public CashTransfer(Entity from, Entity to, float amount, bool iscomplete = false)
    {
        From = from;
        To = to;
        Amount = amount;
        isComplete = iscomplete;
    }

    public void Transfer(Entity to)
    {
        From.Account.Transfer(To.Account, Amount);
        isComplete = true;
    }
}




