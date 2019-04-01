using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class LotMap
{
    Dictionary<Lot,float> Lots = new Dictionary<Lot, float>();

    public void Set(Lot l, float f) {
        Lots[l] = f;
    }


}
