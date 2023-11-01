using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FPSSubTool 
{
    protected FPSTool _fpsMode;
    public virtual void Enable(FPSTool fpsMode) { _fpsMode = fpsMode; }
    public virtual void Disable() { }
    public virtual void ToolInput() { }
}
