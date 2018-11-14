using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ReGoap.Unity;

public class Bootstrap : MonoBehaviour {

    Bank Bank;
    Entity City;

    public Person NewPerson(string name)
    {
        var po = Instantiate(Resources.Load("Person")) as GameObject;
        Person np = po.GetComponent<Person>();
        np.name = name;
        return np;
    }

    // Use this for initialization
    void Start () {
        City = new Entity(name: "City");
        var Lots = GameObject.Find("Blocks").GetComponentsInChildren<Lot>().ToList();
        Lots.ForEach(x => City.AcquireAsset(x));



        var actions = ActionManager.Instance.Actions;

        ConstructionTest();
    }

    void EqualityTest()
    {
        //Debug.Log(PropertyComparer<Entity>.Instance.Equals(Guy1, Guy2, new string[] { "Name" }));
    }

    void ConstructionTest()
    {
        var reqs = BuildingConstructionAction.SkillReqs;


        Person Richie = NewPerson("Richie");

        Person Buck = NewPerson("Buck");
        Buck.AssignUnit(Buck.Entity.WorkUnits[0]);

        Building newBuilding = new Building("new bld", Richie.Entity, (Lot)City.Assets[0], fls: 2, sqf: 200);
        var project = newBuilding.CreateProject();

        var action = Buck.AddComponent<BuildingConstructionAction>();
        var workSearch = Buck.AddComponent<WorkerSearchAction>();
        Buck.AddGoal("hasAsset", project.Deliverable as Building);
        Buck.GetAgent().addAction(workSearch);
        Buck.GetAgent().addAction(action);
        action.AddProject(project);
        workSearch.AddProject(project);

        Person Fred = NewPerson("Fred");

        Fred.AddSkill(SkillType.BldFraming, 1f);
        Fred.AddSkill(SkillType.BldFinishing, 1f);

        Person Bill = NewPerson("Bill");

        Bill.AddSkill(SkillType.BldFoundation, 2f);
    }

	void Update () {
		
	}
}


