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

    DeveloperSensor dev;

    public IndustrySensor() {
    }


    public Neighborhood PriorityArea;
    public Lot TargetLot;

    public override void Sense() {
        if(Officer == null) {
            Officer = GetComponent<Person>();
        }
        if(dev == null) {
            dev = GetComponent<DeveloperSensor>();
        }

        Officer.AddGoal("ProduceSteel", true);
        Officer.AddGoal("developUseSqft", new Tuple<Use, float>(Use.Industrial, 10000));
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

    public void LotEval(Lot l) {

    }
}
