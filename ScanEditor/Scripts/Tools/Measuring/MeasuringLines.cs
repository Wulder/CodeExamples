using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public static class MeasuringLines 
{

    private static List<MeasuringLine> _lines = new List<MeasuringLine>();

    public static void AddLine(Measuring m)
    {
        Vector3 pos = m.p0 + (m.p1 - m.p0) / 2;
        MeasuringLine line = GameObject.Instantiate(GetPrefab(),Vector3.zero,Quaternion.identity).GetComponent<MeasuringLine>();
        line.Canvas.transform.position = pos;
        line.SetPoints(m.p0, m.p1);
        _lines.Add(line);
    }
    
    public static void RemoveLastLine()
    {
        if (_lines.Count == 0) return;
        GameObject.Destroy(_lines.Last().gameObject);
        _lines.Remove(_lines.Last());
    }

    public static void Clear()
    {
        while (_lines.Count > 0) { RemoveLastLine(); }
    }

    static GameObject GetPrefab()
    {
        return Resources.Load("Tools/MeasuringLine", typeof(GameObject)) as GameObject;
    }

}


public struct Measuring
{
    public Vector3 p0, p1;
}
