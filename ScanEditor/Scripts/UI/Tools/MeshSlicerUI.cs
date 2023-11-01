using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlicerUI : ToolUI<MeshSlicerTool>
{
    public void SetUpThreshold(float val)
    {
        _tool.SetUpThreshold(val);
    }

    public void SetDownThreshold(float val)
    {
        _tool.SetDownThreshold(val);
    }
}
