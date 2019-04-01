using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Linq;

public class Entity : IAsset, ITemporal {

    public string Name { get; set; }
    public string ID { get; private set; }
    public List<IAsset> Assets = new List<IAsset>();
    public List<IAsset> Liabilities = new List<IAsset>();
    public Account Account;


    public List<Contract> Contracts = new List<Contract>();
    public List<Project> Projects = new List<Project>();

    public List<WorkUnit> WorkUnits = new List<WorkUnit> {};

    public Person Officer;
    public List<Person> Owners;
    public Entity OwningEntity { get; set; }
    public string Class { get; set; }
    public float LastSalePrice { get; set; }
    public float ValueToOwner { get; set; }
    public List<Tuple<Entity, Person, float, float>> Valuations { get; set; }


    public Entity(string name)
    {
        Class = "Entity";
        GameManager.Instance.Entities.Add(this);
        Name = name;
        AddTemporal();
        ID = GameManager.Instance.Entities.Count.ToString();
        WorkUnits.Add(new WorkUnit(this));
    }

    public virtual void GrantActionsTo(Person p) {

    }

    public void RevokeActionsFrom(Person p) {

    }

    public float CashOnHandTarget;

    public float CashOnHand;

    public virtual float GetValue()
    {
        float value = 0f;
        Assets.ForEach(x => value += x.GetValue());
        Liabilities.ForEach(x => value -= x.GetValue());
        return value;
    }

    public float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }

    Dictionary<IAsset, Sensor> AssetSensorValueDict = new Dictionary<IAsset, Sensor>();

    public virtual void ReceiveAssetValueFromSensor(Sensor sensor, IAsset asset, float val) {
        if(AssetSensorValueDict[asset] == sensor || val > asset.ValueToOwner) {
            asset.ValueToOwner = val;
            AssetSensorValueDict[asset] = sensor;
        }
    }

    public virtual void AcquireAsset(IAsset asset)
    {
        Assets.Add(asset);
        asset.GrantActionsTo(Officer);
        asset.OwningEntity = this;

    }

    public virtual void DivestAsset(IAsset asset)
    {
        Assets.Remove(asset);
    }

    public virtual void DischargeLiability(IAsset liability)
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
        GameManager.Instance.DayTickEvent.AddListener(DayTickListener);
        GameManager.Instance.MonthTickEvent.AddListener(MonthTickListener);
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




