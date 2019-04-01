using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IRandom : MonoBehaviour {

}

public interface ITransferable : ICondition
{
    void Transfer(Entity to);
}

public interface ICondition
{

}

public interface IProjectable
{
    string Name { get; set; }
    Project CreateProject();
    void Complete();
}

public interface IWorkUnitAction
{
    Entity Entity { get; set; }
    WorkUnit WorkUnit { get; set; }
    void Init(WorkUnit workunit);
}

public interface IAsset : ITransferable
{
    string Name { get; set; }
    Entity OwningEntity { get; set; }
    string Class { get; set; }
    float LastSalePrice { get; set; }
    float ValueToOwner { get; set; }
    float GetValue();
    float ValueAccordingTo(Person p);

    //entity, person, timestamp, value
    List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    void GrantActionsTo(Person p);
    void RevokeActionsFrom(Person p);
}

public interface IUnit : IAsset
{
    string Address { get; set; }
}

public interface IProduct : IProjectable {

}

public interface IGood : IProjectable {
    Entity OwningEntity { get; set; }
    List<WorkReq> Reqs { get; set; }
}

public interface ICommodity {

}

public interface ITemporal
{
    void AddTemporal();
    void DayTick();
    void MonthTick();
}

public class TemporalBase : ITemporal
{
    public virtual void AddTemporal()
    {
        var temporal = new Temporal(this);
    }

    public virtual void DayTick()
    {
    }

    public virtual void MonthTick()
    {
    }
}