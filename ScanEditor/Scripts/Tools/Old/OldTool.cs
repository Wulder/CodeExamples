using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class OldTool : MonoBehaviour
{
    [SerializeField] private GameObject _toolUI;
    public virtual void Disable()
    {
        if (_toolUI != null)
            _toolUI.SetActive(false);
        enabled = false;
    }

    public virtual void Enable()
    {
        if (_toolUI != null)
            _toolUI.SetActive(true);
        enabled = true;
    }
}
public struct Triangle
{
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public Triangle(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        p1 = P1;
        p2 = P2;
        p3 = P3;
    }

    public Vector3 GetMidPoint()
    {
        return (p1 + p2 + p3) / 3;
    }
}

