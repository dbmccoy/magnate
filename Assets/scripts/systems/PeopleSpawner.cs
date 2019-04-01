using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour, ITemporal
{
    public void AddTemporal() {
        var v = new Temporal(this);
    }

    public void DayTick() {
    }

    public void MonthTick() {
        Spawn();
    }

    public int ppl;

    public Person Spawn() {
        return NewPerson(GameManager.Instance.People.Count.ToString());
    }

    public Person NewPerson(string name) {
        var po = Instantiate(Resources.Load("Person")) as GameObject;
        Person np = po.GetComponent<Person>();
        np.Name = name;
        po.name = name;
        return np;
    }


    // Start is called before the first frame update
    void Start()
    {
        AddTemporal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
