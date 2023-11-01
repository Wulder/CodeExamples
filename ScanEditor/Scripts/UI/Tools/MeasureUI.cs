using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureUI : ToolUI<MeasurerTool>
{
    public void DeleteLastLine()
    {
        MeasuringLines.RemoveLastLine();
    }

    public void Clear()
    {
        MeasuringLines.Clear();
    }
}
