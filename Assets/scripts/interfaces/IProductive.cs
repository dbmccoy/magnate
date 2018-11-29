using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProductive : ITemporal {

    WorkUnit CurrentUnit { get; set; }
    Project Project { get; set; }

    List<Skill> Skills { get; set; }
	float Capacity { get; set; }

    Skill GetSkill(SkillType type);

    void AssignUnit(WorkUnit unit);

    void AssignProject(Project project);

    void DoWork();
}
