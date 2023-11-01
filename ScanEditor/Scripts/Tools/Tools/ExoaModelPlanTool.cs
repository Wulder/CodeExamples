using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoaModelPlanTool : Tool
{
    private GameObject _floorDesign;

    public override void Enable()
    {
        var gm = Resources.Load("Tools/FloorDesign", typeof(GameObject)) as GameObject;
        _floorDesign = GameObject.Instantiate(gm);
    }

    public override void Disable()
    {
        GameObject.Destroy( _floorDesign );
    }
}
