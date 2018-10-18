using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract : ICondition, ITemporal
{
    public Entity Entity1, Entity2;
    public List<ContractTerm> E1_Terms, E2_Terms;

    public Contract(Entity E1, Entity E2, List<ContractTerm> T1 = null
                    , List<ContractTerm> T2 = null)
    {
        Entity1 = E1;
        Entity2 = E2;
        E1_Terms = T1;
        E2_Terms = T2;
    }

    bool E1_Signed = false;
    bool E2_Signed = false;

    public void Accept(Entity entity)
    {
        if (entity == Entity1)
        {
            E1_Signed = true;
        }
        if (entity == Entity2)
        {
            E2_Signed = true;
        }
        if (E1_Signed && E2_Signed)
        {
            Execute();
        }
    }

    public void Execute()
    {
        Entity1.Contracts.Add(this);
        Entity2.Contracts.Add(this);
    }

    public void Counter()
    {

    }

    public void Reject()
    {

    }

    public void AddTerm(Entity calling, ContractTerm term)
    {
        if (calling == Entity1) E2_Terms.Add(term);
        if (calling == Entity2) E1_Terms.Add(term);
    }

    public void StrikeTerm()
    {

    }

    public Entity OtherParty(Entity calling) { return calling == Entity1 ? Entity2 : Entity1; }
    public Entity SelfParty(Entity calling) { return calling == Entity1 ? Entity1 : Entity2; }
    public List<ContractTerm> SelfTerms(Entity calling){ return calling == Entity1 ? E1_Terms : E2_Terms;}
    public List<ContractTerm> OtherTerms(Entity calling) { return calling == Entity1 ? E2_Terms : E1_Terms; }

    public void AddTemporal()
    {
        var temporal = new Temporal(this);
    }

    public void DayTick()
    {
    }

    public void MonthTick()
    {

        throw new System.NotImplementedException();
    }
}

