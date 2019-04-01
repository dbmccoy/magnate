using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingSensor : Sensor {
    
    protected override void Awake() {
        base.Awake();
        //RentalBullitin.Instance.AddRentalToBullitinEvent.AddListener(EvaluateUnit);
    }

    public override void Sense() {

    }

    public override HashSet<KeyValuePair<string, object>> ReturnWorldData() {
        var data = new HashSet<KeyValuePair<string, object>>();

        return data;
    }

    private void EvaluateUnit(UnitListing listing) {

    }

    public override float EvaluateAsset(IAsset asset) {
        return 0f;
    }
}
