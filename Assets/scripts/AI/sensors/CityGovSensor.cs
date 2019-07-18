using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CityGovSensor : Sensor
{
    public Person Officer;
    SellAssetAction sellAction;

    public void OnAssetSold(IAsset a) {
        if(a == assetToSell) {
            assetToSell = null;
        }
    }

    public override void Sense() {
        if(Entity == null) {
            Entity = GameManager.Instance.city;
        }

        

        Officer.AddGoal("increaseTaxBase", true);

        if(Entity.CashOnHand < Entity.CashOnHandTarget) {
            Officer.AddGoal("gainMoney", true);
            if(assetToSell == null) {
                assetToSell = ChooseAssetToSell();
            }
            else if( Entity.Assets.Count > 0) {
                AssetListing listing = new AssetListing(assetToSell, Officer, assetToSell.GetValue());
                AssetBullitin.Instance.Add(listing);
                sellAction.AddListing(listing);
                assetToSell = null;
            }
        }
        else {
            Officer.RemoveGoal("gainMoney", true);
        }
    }

    public override HashSet<KeyValuePair<string, object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        return data;
    }

    IAsset assetToSell;

    public IAsset ChooseAssetToSell() {
        //calc effect on income from divesting
        var assets = Entity.Assets.OrderBy(i => i.GetValue() - i.ValueToOwner);

        assetToSell = assets.Where(x => AssetBullitin.Instance.Query(x) == null).FirstOrDefault();
        return assetToSell;
    }

    public override float EvaluateAsset(IAsset asset) {

        float value = 0f;
        float timestamp = GameManager.Instance.GetTimeStamp();

        var vals = asset.Valuations;

        value = asset.GetValue();

        if(asset is Lot l) {

        }

        if(asset is Building b) {

        }

        asset.Valuations.Add(new Tuple<Entity, Person, float, float>(Entity, person, GameManager.Instance.GetTimeStamp(), value));

        Evaluations.Add(new System.Tuple<IAsset, float, float>(asset, timestamp, value));

        Entity.ReceiveAssetValueFromSensor(this, asset, value);

        return value;
    }

    // Start is called before the first frame update
    void Start()
    {
        Officer = GetComponent<Person>();
        sellAction = GetComponent<SellAssetAction>();
        sellAction.OnSellAsset.AddListener(OnAssetSold);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override List<LotMap> GetLotMaps() {
        return null;
    }


    //goals:
    //DevelopArea
    //LandBankArea
    //IncreaseTaxBase
    //DevelopAffordableHousing
    //DecreaseCongestion
    //DevelopTransit
}
