using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class IndustrySensor : Sensor
{
    public Person Officer;

    public List<Factory> Factories = new List<Factory>();
    public List<IGood> ProducingGoods = new List<IGood>();

    public LotMap DesireMap = new LotMap();

    public override List<LotMap> GetLotMaps() {
        return new List<LotMap> { DesireMap };
    }

    DeveloperSensor dev;

    public IndustrySensor() {
        DesireMap.Name = "DesireMap";
    }


    public Neighborhood PriorityArea;
    public Lot TargetLot;
    public List<Lot> TargetLots = new List<Lot>();

    public override void Sense() {
        if(Officer == null) {
            Officer = GetComponent<Person>();
        }
        if(dev == null) {
            dev = GetComponent<DeveloperSensor>();
        }

        //sensor decides to produce steel and makes a request for the required sqft of industrial use.  Dev sensor 

        Officer.AddGoal("ProduceSteel", true);
        Officer.AddGoal("developUseSqft", new Tuple<Use, float>(Use.Industrial, 10000));
        //var factory = CreateFactory(10000);
        if(TargetLots.Count == 0) {

            foreach (var p in DesireMap) {
                LotEval(p.lot);
            }

            TargetLots = (DesireMap - dev.CostMap).Where(x => x.lot.BuildableSquareFeet() >= 1000 && x.val > 0).OrderByDescending(x => x.val).Select(x => x.lot).ToList();
        }

        //factory is cached for developaction to pursue unless a suitable location can be bought

        //Officer.Addgoal("hasAsset", factory);
    }

    public override float EvaluateAsset(IAsset asset) {
        return 0f;
    }

    public override HashSet<KeyValuePair<string, object>> ReturnWorldData() 
    {
        var data = new HashSet<KeyValuePair<string, object>>();

        return data;
    }

    List<Neighborhood> Neighborhoods;
    Dictionary<Neighborhood, float> NHoodCurrentEval = new Dictionary<Neighborhood, float>();
    Dictionary<Neighborhood, float> NHoodEvalChange = new Dictionary<Neighborhood, float>();
    Dictionary<Neighborhood, float> NHoodPriority = new Dictionary<Neighborhood, float>();
    public List<Lot> AvailableLots = new List<Lot>();
    public List<float> LotAcquisitionCost = new List<float>();

    DevelopAction devAct;
    CommissionProjectAction commishAct;

    public void NHoodEval(Neighborhood n) {
        float lastEval = NHoodCurrentEval.ContainsKey(n) ? NHoodCurrentEval[n] : 0f;
        NHoodCurrentEval[n] = n.AvgLandVal(); //add in subjective fuzzies
        NHoodEvalChange[n] = NHoodCurrentEval[n] - lastEval;

        NHoodPriority[n] = NHoodCurrentEval[n] + NHoodEvalChange[n];
    }

    float coef_sqft = .1f;
    float coef_port_dist = -10f;

    public void LotEval(Lot l) {
        var v = (l.SquareFeet * coef_sqft) + (l.DistanceTo(GameManager.Instance.GetComponent<Bootstrap>().Port) * coef_port_dist);



        //Debug.Log(v);

        DesireMap.Set(l, v);
    }
}
