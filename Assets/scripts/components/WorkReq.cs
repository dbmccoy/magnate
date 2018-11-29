using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkReq {
    public SkillType Type; //skill type required
    public Skill Skill;
    public float QualityBonus;
    public int Order; //if above 0, workreqs below this value must be completed before contributing
    public float Overhead; //non labor costs per unit of work
    public float RequiredAmount; //required units of work to complete
    public float MaximumAmount; 
    public float CurrentAmount = 0f; 
    public float DeteriorationRate; //how quickly the current amount decreases

    public WorkReq(SkillType type, float reqAmt, float qual = 1f, float maxAmt = 9999f
        , float det = 0f, float qualBonus = 2f, float overhead = 0f, int order = 0)
    {
        Type = type; RequiredAmount = reqAmt; MaximumAmount = maxAmt; Order = order;
        QualityBonus = qualBonus; DeteriorationRate = det; Overhead = overhead;
        Skill = new Skill(type, qual);
    }

    public float PercentComplete()
    {
        return CurrentAmount / RequiredAmount;
    }

    public void TakeInput(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        var excessLevel = skill.value - Skill.value;
        CurrentAmount += 1 + (excessLevel * QualityBonus);
        //Debug.Log(Type + " : " + excessLevel * QualityBonus + " work added : " + CurrentAmount + "/" + RequiredAmount + " completed" );
        //overhead not implemented
    }
}
