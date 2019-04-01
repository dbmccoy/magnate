using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AcquireResidenceAction : GoapAction {

    private Person person;

    //add Unit here
    private bool done;

    public UnitListing Target;
    List<UnitListing> applied = new List<UnitListing>();

    public AcquireResidenceAction()
    {
        addEffect("hasResidence", true);
    }

    public void Awake()
    {
        person = GetComponent<Person>();
    }

    RealEstateSensor myRealEstate;

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        done = false;

        //TODO: has enough money

        if(myRealEstate == null) {
            myRealEstate = person.GetComponent<RealEstateSensor>();
        }

        return RentalBullitin.Instance.Available.Count > 0 || (myRealEstate != null && myRealEstate.myUnits.Count > 0);
    }

    public override bool isDone()
    {
        return done;
    }

    public override bool perform(GameObject agent)
    {
        if (!isDone())
        {
            if(myRealEstate != null && myRealEstate.myUnits.Count > 0) {
                Complete(myRealEstate.myUnits[0]); //TODO: pull unit from market
                RentalBullitin.Instance.Remove(RentalBullitin.Instance.Search(myRealEstate.myUnits[0]));
            }

            if(RentalBullitin.Instance.Available.Count == 0) {
                return false;
            }

            var available = RentalBullitin.Instance.Available.Where(x=> !applied.Contains(x)).ToList();

            var candidates = new List<UnitListing>();
            var ranks = new List<float>();

            foreach (var listing in available)
            {
                candidates.Add(listing);
                ranks.Add(listing.Price);
            }

            if(candidates.Count > 0)
            {
                Target = candidates[ranks.IndexOf(ranks.Min())];
                Target.Apply(person);
            }
        }
        return true;
    }

    public void Complete(UnitListing listing)
    {
        //TODO: send the money
        //TODO: enter into contract
        person.Residence = listing.Unit;
        Debug.Log(person.Name + " moving into " + person.Residence.Address);
        applied.ForEach(x => x.WithdrawApplication(person));
        applied.Clear();
        person.RemoveGoal("hasResidence", true);
        done = true;
    }

    public void Complete(Unit unit) {
        person.Residence = unit;
        Debug.Log(person.Name + " moving into " + person.Residence.Address);
        person.GetComponent<RealEstateSensor>().rentedUnits.Add(unit);
        person.RemoveGoal("rentOut" + unit.Address, unit);
        person.RemoveGoal("hasResidence", true);
        done = true;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override void reset()
    {
    }
}
