using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offer : ITemporal
{
    public Entity Initiator;
    public List<ICondition> This;

    public Entity Responder;
    public List<ICondition> ForThat;

    public int DaysRemaining;
    private readonly int validFor;

    public Offer(Entity init, List<ICondition> _this
                 , Entity resp, List<ICondition> _forThat, int _validFor)
    {
        Initiator = init;
        This = _this;
        Responder = resp;
        ForThat = _forThat;
        validFor = _validFor;
    }



    public void DayTick()
    {

    }

    public void MonthTick()
    {

    }

    public void AddTemporal()
    {
        var temporal = new Temporal(this);
    }
}