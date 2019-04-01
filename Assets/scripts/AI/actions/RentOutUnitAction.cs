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
        addEffect("gainIncome", true);
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
        complete = false;
        Entity = person.CurrentUnit.Entity;

        //TODO: make this faster
        addEffect("hasResidence", true);

        foreach (var goalSet in person.GoalQueue)
        {
            foreach (var goal in goalSet)
            {
                if (goal.Key.StartsWith("rentOut"))
                {
                    if (!GetComponent<RealEstateSensor>().listedUnits.Has((Unit)goal.Value)
                        && !GetComponent<RealEstateSensor>().rentedUnits.Has((Unit) goal.Value)){
                        Unit = (Unit)goal.Value;
                        inProgress = false;
                        addEffect("rentOut" + Unit.Address, Unit);
                        return true;
                    }
                    //TODO: addEffect("getMoney", unit value from sensor)
                }
            }
        }

        return false;
    }

    public override bool isDone()
    {
        if (complete) {
            applicants.Clear();
            person.RemoveGoal("rentOut"+Unit.Address, Unit);
            Unit = null;
            Listing = null;
        }
        return complete;
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
            RentalBullitin.Instance.Remove(Listing);
            topCandidate.GetComponent<AcquireResidenceAction>().Complete(Listing);
            GetComponent<RealEstateSensor>().rentedUnits.Add(Listing.Unit);
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
