using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignAction : GoapAction, IProjectAction
{
    private Person person;

    public Project tempProject { get; set; }
    BuildingDesign design;

    public static HashSet<SkillType> SkillReqs = new HashSet<SkillType>
    {
          SkillType.BldDesign
    };

    public override bool checkProceduralPrecondition(GameObject agent) {
        if(tempProject == null) {
            //return false;
        }

        if(person.Project != null && person.Project.Deliverable is BuildingDesign bd) {

            tempProject = bd.GetProject();

            foreach (var kvp in tempProject.effects) {
                //Debug.Log(kvp.ToString());
            }

            design = bd;

            addEffect(tempProject + "complete", true);

            foreach (var r in tempProject.prereqs) {
                //addPrecondition(r);
            }

        }

        return true;
    }

    public override bool isDone() {
        if (design.isComplete) {
            design = null;
            tempProject = null;
            Debug.Log("nulling design action stuff");
            return true;
        }
        return false;
    }

    public override bool perform(GameObject agent) {
        if(tempProject != null) {
            Debug.Log("design done");
            person.RemoveGoal(tempProject + "complete", true);
            design.Complete();
        }
        return true;
    }

    public override bool requiresInRange() {
        return false;
    }

    public override void reset() {
    }

    // Start is called before the first frame update
    void Awake()
    {
        person = GetComponent<Person>();
        //addPrecondition("hasLotForDev", true);
        addEffect("hasBldDesign", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
