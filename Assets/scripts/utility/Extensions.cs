using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static HashSet<T> ToHashSet<T>(
        this IEnumerable<T> source,
        IEqualityComparer<T> comparer = null)
    {
        return new HashSet<T>(source, comparer);
    }
}
