using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extensions
{
    public static bool HasPair(this HashSet<KeyValuePair<string, object>> h, string s, object o)
    {
        if (h.Contains(new KeyValuePair<string, object>(s, o)))
            return true;
        else return false;
    }

    public static void Add(this HashSet<KeyValuePair<string, object>> h, string s, object o)
    {
        h.Add(new KeyValuePair<string, object>(s, o));
    }

    public static T Random<T>(this List<T> l) {
        if(l.Count > 0) {
            return l[UnityEngine.Random.Range(0, l.Count)];
        }

        return default(T);
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static HashSet<T> ToHashSet<T>(
        this IEnumerable<T> source,
        IEqualityComparer<T> comparer = null)
    {
        return new HashSet<T>(source, comparer);
    }

    public static bool Has(this ICollection col, object obj) {
        foreach (var item in col) {
            if (item.Equals(obj)) {
                return true;
            }
        }
        return false;
    }
}
