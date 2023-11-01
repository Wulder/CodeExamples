using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plan : MonoBehaviour
{
    protected Vector3 _positionOnPlane;
    public Vector3 PositionOnPlane => _positionOnPlane;

    public void UpdatePositionOnPlane(Vector3 newPos)
    {
        _positionOnPlane = newPos;
    }
    public virtual void PlanInput()
    {

    }

    public virtual void ShowUI() { }
    public virtual void HideUI() { }
}
