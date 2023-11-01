using System;
using UnityEngine;
using UnityEngine.VFX;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct Point
{
    public Vector3 Position;
    public Color Color;
    public int X;
    public int Y;
    public int RawDepthValue;
    public int ProcessedDepthValue;
}


unsafe public struct PointData
{
    public Point Point;

    public int Hash;

    public fixed int lvls[7];

    public int PointIndex;

    public Vector3 LocalPos;

}




