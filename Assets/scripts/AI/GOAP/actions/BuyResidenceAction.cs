using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyResidenceAction : GoapAction
{
    private bool done;

    public BuyResidenceAction()
    {
        addEffect("test", true);
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return true;
    }

    public override bool isDone()
    {
        return done;
    }

    public override bool perform(GameObject agent)
    {
        if (!done)
        {
            Debug.Log("buy");
            done = true;
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
        done = false;
    }
}

