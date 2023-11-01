using System;
using System.Diagnostics;
using System.Drawing;
using UnityEngine.VFX;

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct RGB
{
    public RGB(uint r, uint g, uint b)
    {
        R = r;
        G = g;
        B = b;
    }

    public uint R;
    public uint G;
    public uint B;   
}

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct RGB32
{
    public RGB32(byte[] colorBits)
    {
        color = BitConverter.ToUInt32(colorBits, 0);
    }

    public uint color;
}






