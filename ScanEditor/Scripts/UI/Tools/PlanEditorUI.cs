using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanEditorUI : ToolUI<PlanEditorTool>
{

    public void CreateWalsPlan()
    {
        _tool.CreatePlan(PlanType.Walls);
    }

    public void CreateRectanglesPlan()
    {
        _tool.CreatePlan(PlanType.Rectangles);
    }
}
