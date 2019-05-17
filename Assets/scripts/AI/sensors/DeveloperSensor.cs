using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DeveloperSensor : Sensor {

    List<Neighborhood> Neighborhoods;
    Dictionary<Neighborhood, float> NHoodCurrentEval = new Dictionary<Neighborhood, float>();
    Dictionary<Neighborhood, float> NHoodPotentialEval = new Dictionary<Neighborhood, float>();
    Dictionary<Neighborhood, float> NHoodEvalChange = new Dictionary<Neighborhood, float>();
    Dictionary<Neighborhood, float> NHoodPriority = new Dictionary<Neighborhood, float>();
    public List<Lot> AvailableLots = new List<Lot>();
    public List<float> LotAcquisitionCost = new List<float>();

    public LotMap CostMap = new LotMap();

    public override List<LotMap> GetLotMaps() {
        return new List<LotMap>{ CostMap};
    }

    DevelopAction devAct;
    CommissionProjectAction commishAct;

    public List<Tuple<Use,float>> desiredUses = new List<Tuple<Use, float>>();

    public void Start() {
        person = GetComponent<Person>();
        AssetBullitin.Instance.AddAssetToBullitinEvent.AddListener(OnAssetListingAdded);
        devAct = GetComponent<DevelopAction>();
        commishAct = GetComponent<CommissionProjectAction>();
        CostMap.Name = "CostMap";

        CalculateCostMap(); //tie this to an event
    }

    public Neighborhood Priority;
    public Lot TargetLot;

    public List<Project> Projects = new List<Project>();

    public void AddProject(Project p) {
        Projects.Add(p);
        if(p.Deliverable is Building b){

            CostMap.Set(b.Lot, b.Lot.ValueAccordingTo(person) + b.ValueAccordingTo(person));
        }
    }

    public void RequestLot(Use u, float f, Sensor s) {

    }

    public void OnAssetListingAdded(AssetListing a) {
        if(a.Asset is Lot l) {
           CostMap.Set(l, a.Price);
        }
    }

    public void CalculateCostMap() {
        foreach (var l in GameManager.Instance.Lots) {
            LotEval(l);
        }
    }

    public override void Sense() {
        Neighborhoods = GameManager.Instance.GetComponent<Bootstrap>().Neighborhoods;
        Neighborhoods.ForEach(x => NHoodEval(x));

        float top = 0f;

        foreach (var n in NHoodPriority.Keys) {
            if(NHoodPriority[n] > top || top == 0f) {
                Priority = n;
            }
        }

        if(Priority != null) {
            person.AddGoal("develop" + Priority.Name, true);

            var l = AvailableLots.Where(x => x.Buildings.Count == 0).ToList().OrderBy(x => LotAcquisitionCost[AvailableLots.IndexOf(x)]).FirstOrDefault();

        }
    }

    public override HashSet<KeyValuePair<string,object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        foreach(Lot l in AvailableLots) {
            data.Add(l.Address + "isBuildable", l.Buildings.Count == 0);
        }

        return data;
    }

    public void NHoodEval(Neighborhood n) {
        float lastEval = NHoodCurrentEval.ContainsKey(n) ? NHoodCurrentEval[n] : 0f;
        NHoodCurrentEval[n] = n.AvgLandVal(); //add in subjective fuzzies
        NHoodEvalChange[n] = NHoodCurrentEval[n] - lastEval;

        NHoodPriority[n] = NHoodCurrentEval[n] + NHoodEvalChange[n];
    }

    public void LotEval(Lot l) {
        CostMap.Set(l,-10 * l.SquareFeet);
        Debug.Log(-10 * l.SquareFeet);
    }

    public override float EvaluateAsset(IAsset asset) {
        return 0f;
    }
}
