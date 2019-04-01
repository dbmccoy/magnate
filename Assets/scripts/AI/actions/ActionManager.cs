using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class ActionManager {

    private static ActionManager instance;

    public static ActionManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ActionManager();
            }
            return instance;
        }
    }

    public ActionManager()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        var results = new List<Type>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.IsSubclassOf(typeof(GoapAction)))
            {
                Actions.Add(type);
            }
            //if (type.IsSubclassOf(typeof(IHaveSkillReq)))
            //{
            //    SkillReqActions.Add((IHaveSkillReq)type);
            //}
        }
    }

    public List<Type> Actions = new List<Type>();

    //public Type GetAction(string s) {
    //    Actions[0].Name
    //}
    //public List<IHaveSkillReq> SkillReqActions = new List<IHaveSkillReq>();
}
