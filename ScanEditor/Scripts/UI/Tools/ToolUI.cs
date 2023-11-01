using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolUI <T> : MonoBehaviour  
{
    protected T _tool;

    public void Init(T tool)
    {
        _tool = tool;
    }
}
