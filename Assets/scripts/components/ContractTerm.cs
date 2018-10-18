using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractTerm
{
    public Entity Entity1, Entity2;
    public Contract Contract;
    public EvaluateInterval EvaluateType;
    public int LengthDays;
    public int DaysLeft;
    public bool Fulfilled;
    public bool inBreach;

    public ICondition Obligation;
    public string[] Conditions;
    public List<ContractTerm> ContingentOn;
    public List<ContractTerm> RequiredFor;

    public ICondition WarrantyRemedy;
    public bool isBreachableCondition;

    public ContractTerm(Entity E1, Entity E2, ICondition obligation, int days, string[] conditions
                        , EvaluateInterval evalType = EvaluateInterval.once
                        , ICondition remedy = null, bool isBreach = false
                        , List<ContractTerm> contingent = null, List<ContractTerm> required = null)
    {
        Entity1 = E1;
        Entity2 = E2;
        LengthDays = days;
        EvaluateType = evalType;
        Obligation = obligation;
        ContingentOn = contingent;
        Conditions = conditions;
        RequiredFor = required;
        WarrantyRemedy = remedy;
        isBreachableCondition = isBreach;
    }

    public bool Evaluate(ICondition deliverable, bool view = false)
    {
        if (PropertyComparer<ICondition>.Instance.Equals
            (deliverable, Obligation, Conditions))
        {
            if (!view)
            {
                inBreach = false;
                if (EvaluateType == EvaluateInterval.once)
                {
                    Fulfilled = true;
                }
            }
            return true;
        }
        else
        {
            if (!view)
            {
                inBreach = true;
                Debug.Log("in breach");
                if (isBreachableCondition)
                {

                }
            }
            return false;
        }
    }

    public enum EvaluateInterval
    {
        once,
        daily,
        monthly,
        yearly
    }
}