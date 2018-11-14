using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RentResidenceAction : GoapAction {

    private Person person;

    //add Unit here
    private bool done;

    public RentResidenceAction()
    {
        addEffect("hasResidence", true);
    }

    public void Awake()
    {
        person = GetComponent<Person>();
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return RentalBullitin.Instance.Available.Count > 0;
    }

    public override bool isDone()
    {
        return done;
    }

    private float timeElapsed;
    private float startTime;

    public override bool perform(GameObject agent)
    {
        if (!isDone())
        {
            if(startTime == 0f)
            {
                startTime = Time.time;
            }
            timeElapsed += Time.time - startTime;
            if(timeElapsed > 5f)
            {
                agent.GetComponent<Person>().Residence = "Apartment";
                person.RemoveGoal("hasResidence", true);
                Debug.Log("renting");
                done = true;
            }
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
