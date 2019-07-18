using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bootstrap : MonoBehaviour {

    Bank Bank;
    Entity City;
    Person Mayor;
    Person Banker;
    Person Developer;
    Person Builder;
    Person Industrialist;
    Person Architect;

    public List<Neighborhood> Neighborhoods = new List<Neighborhood>();
    public List<Lot> Lots = new List<Lot>();
    public Lot Port;
    public Lot Capitol;

    public Person NewPerson(string name)
    {
        var po = Instantiate(Resources.Load("Person")) as GameObject;
        Person np = po.GetComponent<Person>();
        np.Name = name;
        np.Entity.Name = name + "(E)";
        po.name = name;
        return np;
    }

    // Use this for initialization
    void Start () {
        City = new Entity(name: "City");
        GameManager.Instance.city = City;

        Lots = GameManager.Instance.Lots; //GameObject.Find("Blocks").GetComponentsInChildren<Lot>().ToList();

        //idk
        Port = Lots[0];

        var d = new BuildingDesign(City, Use.Industrial, 300000, Port);
        Building portBuilding = d.GetBuilding();
        Port.Building = portBuilding;

        Capitol = Lots.Last();

        var Blocks = GameObject.Find("Blocks").GetComponentsInChildren<Block>().ToList();

        var n1 = new List<Block> { Blocks[0], Blocks[1], Blocks[3],Blocks[5] };
        Neighborhoods.Add(new Neighborhood("one", n1));

        var n2 = new List<Block> { Blocks[2], Blocks[7], Blocks[8] };
        Neighborhoods.Add(new Neighborhood("two", n2));

        Mayor = NewPerson("Mayor");
        Mayor.CurrentEntity = City;
        City.Officer = Mayor;
        Mayor.AddComponent<CityGovSensor>();
        Mayor.Job = new Job(City, City.WorkUnits[0], Mayor.Skills, Mayor);

        /*
        Industrialist = NewPerson("Industrialist");
        Industrialist.AddComponent<IndustrySensor>();
        Industrialist.Job = new Job(Industrialist.Entity, Industrialist.CurrentUnit, Industrialist.Skills, Industrialist);
        Industrialist.AddComponent<CommissionProjectAction>();
        Industrialist.AddComponent<DevelopAction>();
        */

        Architect = NewPerson("Architect");
        Architect.AddComponent<ProjectSensor>();
        Architect.AddComponent<DesignAction>();
        Architect.Job = new Job(Architect.Entity, Architect.CurrentUnit, Architect.Skills, Architect);

        Architect.AddSkill(SkillType.BldDesign, 10f);


        Lots.ForEach(x => City.AcquireAsset(x));

        City.CashOnHandTarget = 10000f;

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
        Buck.Job = new Job(Buck.Entity, Buck.CurrentUnit, Buck.Skills, Buck);
        Richie.Job = new Job(Richie.Entity, Richie.CurrentUnit, Richie.Skills, Richie);

        /*
        Unit unit = new Unit(800, 2);
        Unit unit2 = new Unit(800, 2);
        Unit unit3 = new Unit(800, 2);
        Unit unit4 = new Unit(800, 2);
        */

        //Building newBuilding = new Building("new bld", Richie.Entity, (Lot)City.Assets[0], fls: 2, sqf: 1);
        //newBuilding.Units.AddRange(new List<Unit> { unit, unit2, unit3, //unit4 });

        //var project = newBuilding.CreateProject();

        Richie.AddComponent<DeveloperSensor>();
        Richie.AddComponent<CommissionProjectAction>();
        Richie.AddComponent<DevelopAction>();

        Buck.AddComponent<BuildingConstructionAction>();
        Buck.AddComponent<WorkerSearchAction>();
        Buck.AddComponent<ProjectSensor>();

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
    Project tempProject { get; set; }

}


