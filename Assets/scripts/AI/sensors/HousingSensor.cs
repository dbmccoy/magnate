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

    private void EvaluateUnit(UnitListing listing) {

    }
}
