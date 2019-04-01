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
    public AssetListing Listing;

    public SellActionEvent OnSellAsset = new SellActionEvent();

    public string ToSell;

    public SellAssetAction() {
        addEffect("gainMoney", true);
    }

    public void Awake() {
        Agent = GetComponent<GoapAgent>();
        person = GetComponent<Person>();
    }

    public void SetAsset(IAsset a) {
        Asset = a;
        ToSell = a.Name;
    }

    public override bool checkProceduralPrecondition(GameObject agent) {

        Entity = person.CurrentEntity;


        if (Asset == null) {
            //Debug.Log("asset is null");
            return false;
        }

        //TODO: make this faster
        addEffect("sellAsset", Asset);
        addEffect("divestAssetClass", Asset.Class); //necessary?
        
        return true;
    }

    public override bool isDone() {
        if (complete) {
            Debug.Log("completed the deal");
            bids.Clear();
            bidVals.Clear();
            person.RemoveGoal("sellAsset", Asset);
            OnSellAsset.Invoke(Asset);
            Asset = null;
            ToSell = "n/a";
            Listing = null;
            inProgress = false;
            complete = false;
            return true;
        }
        return false;
    }

    bool inProgress = false;
    bool complete = false;
    List<AssetBid> bids = new List<AssetBid>();
    List<float> bidVals = new List<float>();

    public void AddBid(AssetBid b) {
        bids.Add(b);
        bidVals.Add(EvalBid(b));
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
        if(inProgress == false) {
            Listing = new AssetListing(Asset, person, Asset.ValueToOwner);
            Debug.Log("adding " + Asset.Name + "for " + Asset.ValueToOwner);
            AssetBullitin.Instance.Add(Listing);
            inProgress = true;
        }
        
        if(bidVals.Count > 0) {

            if (bidVals.Max() >= Asset.ValueToOwner) {
                bids[bidVals.IndexOf(bidVals.Max())].Accept(Entity);
                AssetBullitin.Instance.Remove(Listing);
                Debug.Log("accepting");
                complete = true;
            }
            else {
            }
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
