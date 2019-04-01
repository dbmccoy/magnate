using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RentalHousing {
    public Entity OwningEntity { get; set; }
    public Building Building { get; set; }
    public string Name { get; set; }
    public List<IAsset> Assets { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public List<Unit> Units = new List<Unit>();

    public RentalHousing(){
        Name = "RentalHousing";
    }

    public void AddUnit(Unit u) {
        Units.Add(u);
    }
}
