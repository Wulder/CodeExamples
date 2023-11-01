using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

public class RectangleMesh : MonoBehaviour
{
    private Rectangle _rectangle;
    public Rectangle Rectangle => _rectangle;

    public Mesh Mesh { get { return GetComponent<MeshFilter>().mesh; } private set { } }
    public void Init(Rectangle rect)
    {
        _rectangle = rect;

    }

    public void AddHole(RectangleHole hole)
    {
        _rectangle.Holes.Add(hole);
    }

}
