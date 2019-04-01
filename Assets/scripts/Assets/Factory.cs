using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory
{
    public Entity OwningEntity { get; set; }
    public Building Building { get; set; }
    public string Name { get; set; }
    public List<IAsset> Assets { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<IProductive> Workers = new List<IProductive>();
    public List<IProductive> Machines = new List<IProductive>();

    public List<WorkUnit> WorkUnits = new List<WorkUnit>();
    public List<ProductLine> ProductLines = new List<ProductLine>();

    public Factory() {
        Name = "Factory";
    }

    public void AddProductLine(IGood good) {

        ProductLines.Add(new ProductLine(good));

    }
}

public class ProductLine {
    public IGood good;
    public WorkUnit unit;

    public ProductLine(IGood g, WorkUnit u = null, int num = -1) {
        good = g;
        if(u != null) {
            AssignToWorkUnit(u);
        }
    }

    public void AssignToWorkUnit(WorkUnit u) {
        unit = u;
        Project p = good.CreateProject();
        unit.AddProject(p);
    }
}

public class Steel : IGood {

    public Entity OwningEntity { get; set; }
    public List<WorkReq> Reqs { get; set; }

    public string Name { get { return "Steel"; } set { Name = value; } }

    public Steel() {
        Reqs = new List<WorkReq>()
        {
            new WorkReq(SkillType.SteelWorking,10f,1,order:1)
        };
    }

    public void Complete() {

    }

    public Project CreateProject() {

        var project = new Project(OwningEntity, this, Reqs);
        project.effects.Add(OwningEntity.ID + "HasUse", this.Name);
        //project.prereqs.Add(Lot.Address + "isBuildable", true);

        return project;
    }
}

public enum Use {
    Residential,
    Industrial,
    Commercial
}