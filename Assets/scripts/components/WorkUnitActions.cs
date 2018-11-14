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

    public TypeDictionary<HashSet<SkillType>> Actions = new TypeDictionary<HashSet<SkillType>>
    {
        {typeof(BuildingConstructionAction), new HashSet<SkillType>{ SkillType.BldFoundation, SkillType.BldFraming, SkillType.BldFinishing } }
    };
    

    [SerializeField]
    public HashSet<SkillType> FinLendingReqs = new HashSet<SkillType>
        { SkillType.FinLoaning
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
