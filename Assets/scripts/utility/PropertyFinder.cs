using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static class PropertyFinder<T> where T : class {
    public static object GetProperty(T tObj, string propName)
    {
        //Getting Type of Generic Class Model
        Type type = tObj.GetType();

        //We will be defining a PropertyInfo Object which contains details about the class property 
        PropertyInfo[] arrayPropertyInfos = type.GetProperties();

        //Now we will loop in all properties one by one to get value
        foreach (PropertyInfo property in arrayPropertyInfos)
        {
            Debug.Log(property.Name);
            if(propName == property.Name)
            {
                Debug.Log(property.Name + " : " + property.GetValue(tObj));
                return property.GetValue(tObj);
            }
        }
        return null;
    }

}

public class PropertyComparer<T> : IEqualityComparer<T> where T : class
{
    public static PropertyComparer<T> Instance { get; } = new PropertyComparer<T>();

    public bool Equals(T x, T y)
    {
        throw new NotImplementedException();
    }

    public bool Equals(T x, T y, params string[] props)
    {
        foreach(string p in props)
        {
            if(PropertyFinder<T>.GetProperty(x, p) != PropertyFinder<T>.GetProperty(y, p))
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(T obj)
    {
        return obj.GetHashCode();
    }
}
