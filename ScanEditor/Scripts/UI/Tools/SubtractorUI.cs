using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractorUI : ToolUI<MeshSubtractorTool> 
{
    public void Subtract()
    {
        _tool.Substract();
    }

    public void SetFillHoles(bool val)
    {
        _tool.SetFillHoles(val);
    }
}
