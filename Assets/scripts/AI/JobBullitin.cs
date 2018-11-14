using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JobBullitin {

    public List<Job> Available = new List<Job>();

    public void Add(Job job)
    {
        Available.Add(job);
    }

    public void Remove(Job job)
    {
        Available.Remove(job);
    }

    private static JobBullitin instance;

    private JobBullitin() { }

    public static JobBullitin Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new JobBullitin();
            }

            return instance;
        }
    }
}


