using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

public class LotMap : IEnumerable<LotMapPair>
{
    public string Name;

    public List<Lot> Lots = new List<Lot>();
    public List<float> Vals = new List<float>();

    public List<LotMapPair> pairs = new List<LotMapPair>();

    public LotMap() {
        GameManager.Instance.LotMapSyncEvent.AddListener(Sync);
        Sync();
    }

    public void Sync() {
        GameManager.Instance.Lots.ForEach(x => {
            Set(x, 0f, false);
        });
    }

    public void Set(Lot l, float f, bool overwrite = true) {
        if (Lots.Contains(l)) {
            if (overwrite) {
                int i = Lots.IndexOf(l);
                Vals[i] = f;
                pairs[i].Set(l, f);
                //Debug.Log(Vals[Lots.IndexOf(l)] + ":" + Lots.IndexOf(l));
            }
        }
        else {
            Lots.Add(l);
            Vals.Add(f);
            pairs.Add(new LotMapPair(l, f));
        }
    }

    public float Get(Lot l) {
        try {
            return Vals.IndexOf(Lots.IndexOf(l));
        }
        catch {
            return 0f;
        }
    }

    public void Sort() {

    }

    public Lot Min() {
        Lot m = Lots.FirstOrDefault();
        float f = Vals.IndexOf(Lots.IndexOf(m));

        for (int i = 0; i < Lots.Count; i++) {
            if(Vals[i] < f) {
                m = Lots[i];
                f = Vals[i];
            }
        }

        return m;
    }

    public Lot Max() {
        Lot m = Lots.FirstOrDefault();
        float f = Vals.IndexOf(Lots.IndexOf(m));

        for (int i = 0; i < Lots.Count; i++) {
            if (Vals[i] > f) {
                m = Lots[i];
                f = Vals[i];
            }
        }

        return m;
    }

    public Lot Max(int n) {
        Lot m = Lots.FirstOrDefault();
        float f = Vals.FirstOrDefault();

        for (int i = 0; i < Lots.Count; i++) {
            if(Vals[i] > f) {
                m = Lots[i];
                f = Vals[i];
            }
        }

        return m;
    }

    public IEnumerator<LotMapPair> GetEnumerator() {
        return pairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public static LotMap operator+ (LotMap a, LotMap b) {
        LotMap c = new LotMap();

        for (int i = 0; i < a.Lots.Count; i++) {
            c.Vals[i] = a.Vals[i] + b.Get(a.Lots[i]);
        }

        return c;
    }

    public static LotMap operator- (LotMap a, LotMap b) {
        LotMap c = new LotMap();

        for (int i = 0; i < a.Lots.Count; i++) {
            c.Vals[i] = a.Vals[i] - b.Get(a.Lots[i]);
        }

        return c;
    }

    public static LotMap operator* (LotMap a, LotMap b) {
        LotMap c = new LotMap();

        for (int i = 0; i < a.Lots.Count; i++) {
            c.Vals[i] = a.Vals[i] * b.Get(a.Lots[i]);
        }

        return c;
    }

    public static LotMap operator/ (LotMap a, LotMap b) {
        LotMap c = new LotMap();

        for (int i = 0; i < a.Lots.Count; i++) {
            c.Vals[i] = a.Vals[i] / b.Get(a.Lots[i]);
        }

        return c;
    }
}

public class LotMapPair {
    public Lot lot;
    public float val;

    public LotMapPair(Lot l, float v) {
        Set(l, v);
    }

    public void Set(Lot l, float v) {
        lot = l;
        val = v;
    }
}


