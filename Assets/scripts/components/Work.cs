using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SkillType {
    BldFoundation,
    BldFraming,
    BldFinishing,
    BldMaintenance,

    FinLoaning,

    PolCanvas
}

public class Skill
{
    public SkillType type;
    public float value;

    public Skill(SkillType t, float v)
    {
        type = t;
        value = v;
    }

    public static bool operator >=(Skill obj1, Skill obj2)
    {
        if(obj1 is Skill skill1 && obj2 is Skill skill2)
        {
            return (skill1.type == skill2.type && skill1.value >= skill2.value);
        }
        return false;
    }

    public static bool operator <=(Skill obj1, Skill obj2)
    {
        if (obj1 is Skill skill1 && obj2 is Skill skill2)
        {
            return (skill1.type == skill2.type && skill1.value <= skill2.value);
        }
        return false;
    }
}

