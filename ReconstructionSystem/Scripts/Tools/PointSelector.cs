using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PointSelector : Tool
{
    public event Action<RaycastHit> OnSelectedNewPoint;
    public event Action<int> OnSelectedNewPointIndex;
    protected void InvokeNewPoint(RaycastHit hit)
    {
        OnSelectedNewPoint?.Invoke(hit);
    }

    protected void InvokeNewPointIndex(int index)
    {
        OnSelectedNewPointIndex?.Invoke(index);
    }
}
