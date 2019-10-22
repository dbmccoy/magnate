using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BuyAssetAction : GoapAction
{
    private Person person;
    private Entity Entity;

    //add Unit here
    private bool done;

    public AssetListing Target;
    AssetBid bid;
    List<AssetListing> applied = new List<AssetListing>();
    List<AssetListing> blacklist = new List<AssetListing>();

    public BuyAssetAction() {
    }

    public void Awake() {
        person = GetComponent<Person>();
        Entity = person.CurrentEntity;
    }

    public override bool checkProceduralPrecondition(GameObject agent) {
        done = false;


        if (Target == null && person.GetAgent().Goal != null) 
        {
            
            foreach(var a in person.GetAgent().availableActions) {

                foreach (var c in a.Preconditions) {
                    //Debug.Log(GoapAgent.prettyPrint(a) + " has precond " + c.Key + ":" + c.Value.ToString());

                    if(a is CommissionProjectAction cp) {
                        Debug.Log(cp.tempProject.Deliverable);
                    }

                    if (c.Key.Contains("hasAsset")) {

                        Target = AssetBullitin.Instance.Query((IAsset)c.Value);
                        Debug.Log(c.Value);
                    }

                    if (Target != null) {
                        //Debug.Log(GoapAgent.prettyPrint(person.GetAgent().Goal) + a.GetType().ToString() + " " + c.Key + " adding " + Target.Asset.Name);
                        addEffect(c.Key, c.Value);
                        break;
                    }
                }
            }


            if(Target == null) {
                return false;
            }

            addEffect(person.CurrentEntity.ID + "hasAsset", Target.Asset);

        }
        else {
            //Debug.Log(Target.Asset.Name);
        }

        return true;

        //TODO: has enough money

    }

    public override bool isDone() {
        if(bid != null && bid.isAccepted) {
            //Debug.Log("bought a thing " + Target.Asset.Name);

            Target = null;
            bid = null;
            done = true;

        }
        return done;
    }

    public override bool perform(GameObject agent) {
        if(bid == null || bid.isDeclined) {
            MakeOffer();
        }

        return true;
    }

    public void MakeOffer() {

        var p1 = new Tuple<Entity, List<IAsset>, float>(
                Entity,
                new List<IAsset>(),
                Target.Asset.GetValue()
                );

        var p2 = new Tuple<Entity, List<IAsset>, float>(
                Target.OwnedBy,
                new List<IAsset>() { Target.Asset },
                Target.Price
                );

        bid = new AssetBid(p1, p2);
        bid.Accept(Entity);

        Target.ReceiveOffer(bid);
    }

    public void Complete(AssetListing listing) {
       
        done = true;
    }


    public override bool requiresInRange() {
        return false;
    }

    public override void reset() {
    }
}

public class AssetBid {

    public AssetBid(Tuple<Entity, List<IAsset>, float> p1, Tuple<Entity, List<IAsset>, float> p2) {
        party1 = p1;
        party2 = p2;
    }

    Tuple<Entity, List<IAsset>, float> party1;
    Tuple<Entity, List<IAsset>, float> party2;

    float p1interest;
    float p2interest;

    public void SetInterest(Entity p, float v) {
        if(p == party1.Item1) {
            p1interest = v;
        }
        if (p == party2.Item1) {
            p2interest = v;
        }
    }

    public float GetInterest(Entity p) {
        if (p == party1.Item1) {
            return p1interest;
        }
        if (p == party2.Item1) {
            return p2interest;
        }
        else return 0f;
    }

    public Tuple<Entity, List<IAsset>, float> Other(Entity p) {
        if (p == party1.Item1) return party2;
        else return party1;
    }

    public Tuple<Entity, List<IAsset>, float> Self(Entity p) {
        if (p == party1.Item1) return party1;
        else return party2;
    }

    bool party1accepts;
    bool party2accepts;

    bool party1declines;
    bool party2declines;

    public bool isDeclined;
    public bool isAccepted;

    public void Accept(Entity p) {

        if (p == party1.Item1) {

            party1accepts = true;
        }

        if (p == party2.Item1) {
            party2accepts = true;
        }

        if(party1accepts && party2accepts) {

            //escrow??
            isAccepted = true;

            party1.Item2.ForEach(x => x.Transfer(party2.Item1));
            //party1.Item1.Account.Transfer(party2.Item1.Account, party1.Item3);

            //party2.Item2.ForEach(x => x.Transfer(party1.Item1));

            foreach(var x in party2.Item2) {

                //Debug.Log(party1.Item1.Name + " gets " + x.Name);
                x.Transfer(party1.Item1);

            }

            //party2.Item1.Account.Transfer(party1.Item1.Account, party2.Item3);

        }
    }

    public void Decline(Entity e) {
        if (e == party1.Item1) party1declines = true;
        if (e == party2.Item1) party2declines = true;

        isDeclined = true;
    }
}
