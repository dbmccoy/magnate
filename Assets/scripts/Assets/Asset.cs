using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Asset : IAsset {
    public virtual string Name { get; set; }
    public virtual Entity OwningEntity { get; set; }
    public virtual string Class { get; set; }
    public virtual float LastSalePrice { get; set; }
    public virtual float ValueToOwner { get; set; }
    public virtual List<Tuple<Entity, Person, float, float>> Valuations { get; set; }

    public virtual float GetValue() {
        Debug.Log(Name + " unimplemented GetValue(), returning 0");
        return 0;
    }

    public virtual float ValueAccordingTo(Person p) {
        return p.GetSensors().Select(x => x.EvaluateAsset(this)).Max();
    }

    public abstract void GrantActionsTo(Person p);

    public abstract void RevokeActionsFrom(Person p);

    public abstract void Transfer(Entity to);
}
