using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;




public abstract class Tool
{
    
    public virtual void Enable() { }
    public virtual void Disable() { }
    public virtual void ToolInput() { }

    public virtual void DrawGizmos() { }

    public virtual void DrawGUI() { }

    

}

public enum ToolType 
{
    AngleAlign,
    EmptyTool,
    ExoadModelPlan,
    FloorAlign,
    Measurer,
    MeshSlicer,
    MeshSubtractor,
    WallsEditor
}

