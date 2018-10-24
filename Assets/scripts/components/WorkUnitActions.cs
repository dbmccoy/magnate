using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

public sealed class WorkUnitActions {

    private static WorkUnitActions instance = null;
    private static readonly object padlock = new object();

    public static WorkUnitActions Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new WorkUnitActions();
                }
                return instance;
            }
        }
    }



    public WorkUnitActions()
    {

    }

    [SerializeField]
    public HashSet<Work> BuildingConstructionReqs = new HashSet<Work>
        { Work.BldFoundation
        , Work.BldFraming
        , Work.BldFinishing};


    public TypeDictionary<HashSet<Work>> Actions = new TypeDictionary<HashSet<Work>>
    {
        {typeof(BuildingConstructionAction), new HashSet<Work>{ Work.BldFoundation, Work.BldFraming, Work.BldFinishing } }
    };
    

    [SerializeField]
    public HashSet<Work> FinLendingReqs = new HashSet<Work>
        { Work.FinLoaning
        };

}


namespace System.Collections.Specialized
{
    public class TypeDictionary<TValue> : Dictionary<Type, TValue>
    {
        public TValue Get<T>()
        {
            return this[typeof(T)];
        }

        public void Add<T>(TValue value)
        {
            Add(typeof(T), value);
        }

        public bool Remove<T>()
        {
            return Remove(typeof(T));
        }

        public bool TryGetValue<T>(out TValue value)
        {
            return TryGetValue(typeof(T), out value);
        }

        public bool ContainsKey<T>()
        {
            return ContainsKey(typeof(T));
        }
    }
}
