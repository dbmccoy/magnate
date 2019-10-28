using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class SellAssetAction : GoapAction
{
    private GoapAgent Agent;
    private Person person;
    public IAsset Asset { get; private set; }
    public Entity Entity;

    public SellActionEvent OnSellAsset = new SellActionEvent();

    public string ToSell;

    public SellAssetAction() {
        addEffect("gainMoney", true);
    }

    public void Awake() {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
    }

    AssetBid bid;

    public override bool checkProceduralPrecondition(GameObject agent) {

        Entity = person.CurrentEntity;


        if (listings.Count == 0 || bids.Count == 0) {
            //Debug.Log("asset is null");
            return false;
        }

        bid = bids.First();

        if (bids.First().GetInterest(Entity) > 0) {
        }

        //TODO: make this faster
        foreach (IAsset a in bid.Self(Entity).Item2) {
            addEffect("sellAsset", a);
        }
        foreach (IAsset a in bid.Other(Entity).Item2) {
            addEffect("hasAsset", a);
        }

        return true;
    }

    public override bool isDone() {
        if (complete) {
            inProgress = false;
            complete = false;
            return true;
        }
        return false;
    }

    bool inProgress = false;
    bool complete = false;
    public List<AssetListing> listings = new List<AssetListing>();

    List<AssetBid> bids = new List<AssetBid>();
    List<float> bidVals = new List<float>();

    public void AddListing(AssetListing l) {
        listings.Add(l);
    }

    public void RemoveListing(AssetListing l) {
        listings.Remove(l);
    }

    public void AddBid(AssetBid b) {
        bids.Add(b);
        bidVals.Add(EvalBid(b));

        bids.OrderBy(x => x.GetInterest(Entity) > 0);
        //how discard old bids?
    }

    public void RemoveBid(AssetBid b) {
        bidVals.RemoveAt(bids.IndexOf(b));
        bids.Remove(b);
    }

    public float EvalBid(AssetBid b) {
        float val = 0f;

        foreach (var asset in b.Other(Entity).Item2) {
            val += asset.GetValue();
        }
        val += b.Other(Entity).Item3;
        return val;
    }

    public override bool perform(GameObject agent) {

        bid.Accept(Entity);

        if (bid.isAccepted) {
            //Debug.Log("accepting");
            complete = true;
            RemoveBid(bid);
            bid = null;

        }

        return true;
    }

    public override bool requiresInRange() {
        return false;
    }

    public override void reset() {

    }
}

public class SellActionEvent : UnityEvent<IAsset> {

}

public class AssetListing : TemporalBase {
    public IAsset Asset;
    public Person PostedBy;
    public Entity OwnedBy;
    public float Price;
    public int Age;

    public AssetListing(IAsset asset, Person person, float price) {
        Asset = asset;
        PostedBy = person;
        OwnedBy = asset.OwningEntity;
        Price = price;
    }

    public void ReceiveOffer(AssetBid b) {
        PostedBy.GetComponent<SellAssetAction>().AddBid(b);
    }

    public void WithdrawOffer(AssetBid b) {
        PostedBy.GetComponent<SellAssetAction>().RemoveBid(b);
    }

    public override void DayTick() {
        Age += 1;
    }
}
