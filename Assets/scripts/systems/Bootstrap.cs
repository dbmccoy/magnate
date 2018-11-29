using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        Unit unit = new Unit(800, 2);
        Unit unit2 = new Unit(800, 2);

        Building newBuilding = new Building("new bld", Richie.Entity, (Lot)City.Assets[0], fls: 2, sqf: 1);
        newBuilding.Units.AddRange(new List<Unit> { unit });

        var project = newBuilding.CreateProject();

        var commission = Richie.AddComponent<CommissionProjectAction>();

        Richie.AddProject(project);

        var action = Buck.AddComponent<BuildingConstructionAction>();
        var workSearch = Buck.AddComponent<WorkerSearchAction>();

        var sensor = Buck.AddComponent<ProjectSensor>();

        var landlordAction = Richie.AddComponent<RentOutUnitAction>();
        var RE_sensor = Richie.AddComponent<RealEstateSensor>();


        Person Fred = NewPerson("Fred");

        Fred.AddSkill(SkillType.BldFraming, 1f);
        Fred.AddSkill(SkillType.BldFinishing, 1f);

        Person Bill = NewPerson("Bill");

        Bill.AddSkill(SkillType.BldFoundation, 2f);
    }

	void Update () {
		
	}
}

public interface IProjectAction
{
    Project Project { get; set; }

}


