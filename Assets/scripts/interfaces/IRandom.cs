using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Project CreateProject();
}

public interface IWorkUnitAction
{
    Entity Entity { get; set; }
    WorkUnit WorkUnit { get; set; }
    void Init(WorkUnit workunit);
}

public interface IOwnable : ITransferable
{
    string Name { get; set; }
    Entity OwningEntity { get; set; }

    float GetValue();
}

public interface IUnit : IOwnable
{
    string Address { get; set; }
}


public interface ITemporal
{
    void AddTemporal();
    void DayTick();
    void MonthTick();
}