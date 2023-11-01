using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsMode : ButtonCore
{
    protected override void OnPress()
    {
        ApplicationController.Instance._toolManager.SetTool(new FPSTool());
    }
}
