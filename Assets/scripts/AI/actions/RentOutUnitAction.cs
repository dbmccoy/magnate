using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RentOutUnitAction : GoapAction {

    private GoapAgent Agent;
    private Person person;
    private RealEstateSensor RE_Sensor;
    public Unit Unit;
    public Entity Entity;

    public RentOutUnitAction()
    {
        
    }

    public void Awake()
    {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
        RE_Sensor = GetComponent<RealEstateSensor>();
    }


    public override bool checkProceduralPrecondition(GameObject agent)
    {
        Unit = null;
        Entity = person.CurrentUnit.Entity;

        //TODO: make this faster

        foreach (var goalSet in person.GoalQueue)
        {
            foreach (var goal in goalSet)
            {
                if (goal.Key == "rentOutUnit")
                {
                    Unit = (Unit)goal.Value;
                    addEffect("rentOutUnit", Unit);
                    //TODO: addEffect("getMoney", unit value from sensor)
                    return true;
                }
            }
        }

        return false;
    }

    public override bool isDone()
    {
        return false;
    }

    bool inProgress = false;
    bool complete = false;
    List<Person> applicants = new List<Person>();

    public void AddApplicant(Person p)
    {
        applicants.Add(p);
    }

    public void RemoveApplicant(Person p)
    {
        applicants.Remove(p);
    }

    public UnitListing Listing;

    public override bool perform(GameObject agent)
    {
        if (!inProgress)
        {
            Debug.Log("add listing");
            inProgress = true;

            Listing = new UnitListing(Unit, person, 1000); //TODO: get price from sensor

            RentalBullitin.Instance.Add(Listing);
        }
        if(applicants.Count > 0)
        {
            //run fitness test on each applicant
            Person topCandidate = null;
            var candidates = new List<Person>();
            var ranks = new List<float>();

            foreach (var p in applicants)
            {
                candidates.Add(p);
                ranks.Add(Random.Range(0, 10));
            }

            topCandidate = candidates[ranks.IndexOf(ranks.Max())];
            //if topCandate clears fitness bar rent the unit
            //both parties sign lease
            topCandidate.GetComponent<RentResidenceAction>().Complete(Listing);
            complete = true;
        }
        return true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
    }
}
