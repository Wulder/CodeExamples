using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoleFiller
{
    public Mesh Fill(List<Vector3> sortedContourPoints)
    {


        List<int> freePoints = new List<int>();
        HashSet<TriangleIndicies> filledTris = new HashSet<TriangleIndicies>();
        int pointIndex = 0;
        foreach (var contourPoint in sortedContourPoints)
        {
            freePoints.Add(pointIndex++);
        }

        List<Vector3> clockwiseCheck = new List<Vector3>();
        freePoints.ForEach(freePoint => { clockwiseCheck.Add(sortedContourPoints[freePoint]); });
        bool isClockwise = IsContourClockwise(clockwiseCheck.ToArray());
        Vector3 centerOfMass = CalculateCenterOfMass(sortedContourPoints.ToArray());

        while (freePoints.Count >= 3)
        {

            freePoints = FillFreePoints(freePoints, ref filledTris, isClockwise, centerOfMass, sortedContourPoints);

        }

        Mesh filling = new Mesh();
        filling.vertices = sortedContourPoints.ToArray();
        filling.triangles = MeshSplitter.TrisToIntArray(filledTris.ToList());
        Vector3 normal = Vector3.up;

        return filling;
    }


    List<int> FillFreePoints(List<int> freePoints, ref HashSet<TriangleIndicies> tris, bool isClockwise, Vector3 centerOfMass, List<Vector3> pointPos)
    {


        HashSet<int> newFreePoints = new HashSet<int>();

        int currentIndex = 0;
        while (currentIndex < freePoints.Count - 1)
        {
            int i0 = currentIndex;
            int i1 = currentIndex + 1;
            int i2 = currentIndex + 2;

            if (i2 > freePoints.Count - 1)
            {
                i2 = 0;
            }
            // Debug.Log($"{i0} || {i1} || {i2} || {freePoints.Count} - ({string.Join(",", freePoints.ToArray())})");

            Vector3 intepolationPoint = Vector3.Lerp(pointPos[freePoints[i0]], pointPos[freePoints[i2]], 0.5f);
            bool correctAngle = Vector3.Distance(centerOfMass, intepolationPoint) < Vector3.Distance(centerOfMass, pointPos[freePoints[i1]]);


            if (!isClockwise)
            {
                tris.Add(new TriangleIndicies(freePoints[i0], freePoints[i1], freePoints[i2]));
            }
            else
                tris.Add(new TriangleIndicies(freePoints[i2], freePoints[i1], freePoints[i0]));



            newFreePoints.Add(freePoints[i0]);
            newFreePoints.Add(freePoints[i2]);
            currentIndex += 2;
        }


        return newFreePoints.ToList();
    }


    private bool IsContourClockwise(Vector3[] points)
    {
        int numPoints = points.Length;
        float sum = 0f;

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % numPoints];
            Vector3 center = CalculateCenterOfMass(points);

            Vector3 currentDir = current - center;
            Vector3 nextDir = next - center;

            float angle = Vector3.SignedAngle(currentDir, nextDir, center);
            sum += angle;
        }

        return sum > 0f;
    }

    private Vector3 CalculateCenterOfMass(Vector3[] points)
    {
        Vector3 center = Vector3.zero;
        int numPoints = points.Length;

        foreach (var point in points)
        {
            center += point;
        }

        center /= numPoints;
        return center;
    }
}
