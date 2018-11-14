using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RentalBullitin
{

    public List<Unit> Available = new List<Unit>();

    public void Add(Unit unit)
    {
        Available.Add(unit);
    }

    public void Remove(Unit unit)
    {
        Available.Remove(unit);
    }

    private static RentalBullitin instance;

    private RentalBullitin() { }

    public static RentalBullitin Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new RentalBullitin();
            }

            return instance;
        }
    }
	
}
