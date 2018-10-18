using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProductive : ITemporal {

    WorkUnit CurrentUnit { get; set; }
    Project CurrentProject { get; set; }

    Dictionary<Work.Type, float> Skills { get; set; }
	float Capacity { get; set; }

    void AssignUnit(WorkUnit unit);

    void AssignProject(Project project);

    void DoWork();
}
