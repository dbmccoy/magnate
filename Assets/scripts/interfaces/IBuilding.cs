using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding : IAsset, IProjectable {

    int Floors { get; }
    int SquareFeet { get; }
    Lot Lot { get; }
    BuildingObj BuildingObj { get; }
}
