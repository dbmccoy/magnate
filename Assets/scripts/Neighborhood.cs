using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighborhood
{
    public string Name;
    public List<Block> Blocks;

    public Neighborhood(string name, List<Block> blocks) {
        Name = name;
        Blocks = blocks;
    }

    public void AddBlock(Block b) {
        b.SetNeighborhood(this);
    }

    public float AvgLandVal() {

        float val = 0;
        int count = 0;

        foreach (var b in Blocks) {

            b.Lots.ForEach(x => val += x.GetValue());
            count += b.Lots.Count;
        }

        return val / count;
    }
}
