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
    public LotMap ConstCostMap = new LotMap();
    public LotMap DesireMap = new LotMap();

    public override List<LotMap> GetLotMaps() {
        return new List<LotMap>{ CostMap,DesireMap };
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
        DesireMap.Name = "DesireMap";
        ConstCostMap.Name = "ConstCostMap";
        Entity = person.CurrentEntity;

        foreach (var l in GameManager.Instance.Lots) {
            l.OnLotUpdate.AddListener(LotEval);
        }

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

        

        foreach (var item in DesireMap) {
            //Debug.Log(item.val);
        }
    }

    private Project currentProject;

    UseReqs currentReqs;
    bool updatedLots;

    public override void Sense() {
        Neighborhoods = GameManager.Instance.GetComponent<Bootstrap>().Neighborhoods;
        Neighborhoods.ForEach(x => NHoodEval(x));

        float top = 0f;

        foreach (var n in NHoodPriority.Keys) {
            if(NHoodPriority[n] > top || top == 0f) {
                Priority = n;
            }
        }

        if(currentReqs != null) {
            bool inProgress = false;
            foreach (var g in person.FindGoals("developUseSqft")) {
                if(g.Value == currentReqs) {
                    inProgress = true;
                }
            }

            if (!inProgress) {
                Debug.Log("reset");
                currentReqs = null;
            }
        }

        if(currentReqs == null) {
            CalculateCostMap();
            currentReqs = new UseReqs(Use.Residential, DesireMap, 1000);
            person.AddGoal("developUseSqft", currentReqs);

        }

        if (updatedLots) {
            DesireMap.Sort(DesireMap.OrderByDescending(x => x.val));
            updatedLots = false;
        }
    }

    public override HashSet<KeyValuePair<string,object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        foreach(Lot l in AvailableLots) {
            data.Add(l.Address + "isBuildable", l.Building == null);
        }

        return data;
    }

    public void NHoodEval(Neighborhood n) {
        float lastEval = NHoodCurrentEval.ContainsKey(n) ? NHoodCurrentEval[n] : 0f;
        NHoodCurrentEval[n] = n.AvgLandVal(); //add in subjective fuzzies
        NHoodEvalChange[n] = NHoodCurrentEval[n] - lastEval;

        NHoodPriority[n] = NHoodCurrentEval[n] + NHoodEvalChange[n];
    }

    float coef_sqft = .1f;
    float coef_port_dist = -100f;
    float coef_cost = -1f;



    public void LotEval(Lot l) {


        float c = UnityEngine.Random.Range(500, 1000);
        if (l.Building == null) {
            c += 0f;
        }
        else {
            c += 10000f; //demolition costs
        }

        CostMap.Set(l,c);

        float v = (l.SquareFeet * coef_sqft) + (l.DistanceTo(GameManager.Instance.GetComponent<Bootstrap>().Capitol) * coef_port_dist) + (CostMap.Get(l) * coef_cost);

        DesireMap.Set(l, v);

        //Debug.Log(v);

        updatedLots = true;
    }

    public int ConstructCost(Lot l) {

        if(currentReqs == null) {
            return 0;
        }
        int max = (int)l.BuildableSquareFeet();
        int avail = max - l.Building.SquareFeet;

        return (max / avail) * (currentReqs.sqft * 10);
    }

    public override float EvaluateAsset(IAsset asset) {
        return 0f;
    }
}

public class UseReqs {
    public Use use;
    public LotMap map;
    public int sqft;

    public UseReqs(Use u, LotMap l, int s) {
        use = u;
        map = l;
        sqft = s;
    }
}