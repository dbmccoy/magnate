using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RentResidenceAction : GoapAction {

    private Person person;

    //add Unit here
    private bool done;

    public UnitListing Target;
    List<UnitListing> applied = new List<UnitListing>();

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
        //TODO: has enough money
        return RentalBullitin.Instance.Available.Count > 0;
    }

    public override bool isDone()
    {
        return done;
    }

    public override bool perform(GameObject agent)
    {
        if (!isDone())
        {
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
                Debug.Log("rental apply");
                Target.Apply(person);
            }
        }
        return true;
    }

    public void Complete(UnitListing listing)
    {
        //TODO: send the money
        //TODO: enter into contract
        person.Residence = "residence";
        applied.Remove(listing);
        applied.ForEach(x => x.WithdrawApplication(person));
        done = true;
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
