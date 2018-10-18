using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ReGoap.Unity;

public class Bootstrap : MonoBehaviour {

    Entity Capitalist;
    Entity Contractor;
    Entity Guy1;
    Bank Bank;
    Entity City;

	// Use this for initialization
	void Start () {
        City = new Entity(name: "City");
        var Lots = GameObject.Find("Blocks").GetComponentsInChildren<Lot>().ToList();
        Lots.ForEach(x => City.AcquireAsset(x));
        Capitalist = new Entity(name: "Bezos");

        Contractor = new Entity(name: "Buck's Construction");

        Guy1 = new Entity(name: "Joe");
        Bank = new Bank(name: "Bank of Fuck", owner: Capitalist);
        Bank.NewAccount(Guy1);
        Bank.NewAccount(City);
        Bank.NewAccount(Contractor);

        Bank.Deposits.Balance += 2000000;

        PlanTest();

        //ConstructionTest();
    }

    // Update is called once per frame

    void BankTest()
    {
        var loan = new Loan(Bank, Guy1, null, 100000, .03f, 60);
        Bank.IssueLoan(loan);

        //Worker.Account.Transfer(City.Account, 100000);
        //City.TransferAsset(Worker, City.Assets[0]);

        Debug.Log("Joe now owns " + Guy1.Assets[0].Name);
        Debug.Log("Joe has " + Guy1.Account.Balance + " dollars");
    }

    void EqualityTest()
    {
        Entity Guy2 = new Entity(name: "Joe");

        Debug.Log(PropertyComparer<Entity>.Instance.Equals(Guy1, Guy2, new string[] { "Name" }));
    }

    void PlanTest()
    {
        GameObject g1 = Instantiate(Resources.Load("Person")) as GameObject;
        Person jeff = g1.GetComponent<Person>();
        Building building = new Building("new bld", jeff.Entity, (Lot)City.Assets[0], fls: 2, sqf: 200);
        jeff.Projects.Enqueue(building.CreateProject());
        BuildingConstructionAction bldAction = jeff.gameObject.AddComponent(typeof(BuildingConstructionAction)) as BuildingConstructionAction;
        HaveAssetGoal jeffGoal = jeff.gameObject.AddComponent(typeof(HaveAssetGoal)) as HaveAssetGoal;
        jeffGoal.SetGoal(building);
        //PlannerManager.Instance.Plan(jeff.GetAgent(), jeffGoal,null,null);
    }

    void ConstructionTest()
    {
        var reqs = WorkUnitActions.Instance.BuildingConstructionReqs;

        GameObject CE1G = Instantiate(Resources.Load("Person")) as GameObject;
        GameObject CE2G = Instantiate(Resources.Load("Person")) as GameObject;
        Person CE1 = CE1G.GetComponent<Person>();
        Person CE2 = CE2G.GetComponent<Person>();

        CE1.AddSkill(Work.Type.BldFoundation, 2f);
        CE1.AddSkill(Work.Type.BldFraming, 1f);
        CE1.AddSkill(Work.Type.BldFinishing, 1f);

        CE2.AddSkill(Work.Type.BldFoundation, 1f);
        CE2.AddSkill(Work.Type.BldFraming, 3f);
        CE2.AddSkill(Work.Type.BldFinishing, 1f);

        CE1.AssignUnit(Contractor.WorkUnits[0]);
        CE2.AssignUnit(Contractor.WorkUnits[0]);

        Building building = new Building("new bld", Capitalist, (Lot)City.Assets[0], fls: 2, sqf: 200);

        //Project project = building.CreateProject();

        BuildingConstructionAction builder = (BuildingConstructionAction)Contractor.WorkUnits[0].ActionDict[typeof(BuildingConstructionAction)];

        if(builder != null)
        {
            //builder.ConstructBuilding(building);
        }
        else
        {
            Debug.Log("where builder");
        }
    }

	void Update () {
		
	}
}


