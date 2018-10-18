using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReGoap.Unity
{
    public class HaveAssetGoal : ReGoapGoal<string, object>
    {
        public void SetGoal(IOwnable asset)
        {
            goal.Set("hasAsset", asset);
        }
    }
}

