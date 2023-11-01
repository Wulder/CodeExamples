using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAlignUI : ToolUI<FloorAlignTool> 
{
    public void Align()
    {
        
        _tool.AlignMesh();
    }
}
