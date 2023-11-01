using Parabox.CSG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HoleCutter 
{


    public Mesh CutHole(RectangleMesh mesh, RectangleHole hole)
    {
        var subtractor = CreateSubtracter(mesh, hole);
        Model result = CSG.Subtract(mesh.gameObject, subtractor);
        GameObject.Destroy(subtractor);

        Vector3[] newVerts =result.mesh.vertices;

        for (int i = 0; i < result.mesh.vertices.Length; i++)
            newVerts[i] -= mesh.transform.position;

        Mesh m = new Mesh();

        m.vertices = newVerts;
        m.triangles = result.mesh.triangles;
        m.normals = result.mesh.normals;
        
        return m;
    }

    public GameObject CreateSubtracter(RectangleMesh mesh, RectangleHole hole)
    {
        
        var subtractor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var meshBounds = mesh.GetComponent<MeshFilter>().mesh.bounds; 
        subtractor.transform.position = mesh.transform.position + meshBounds.center;

        if (hole.Normal == Vector3.right || hole.Normal == Vector3.left)
        {
            subtractor.transform.position = mesh.transform.position + meshBounds.center + new Vector3(0, hole.Position.y, hole.Position.x);
            subtractor.transform.localScale = new Vector3(meshBounds.size.x, hole.Size.y, hole.Size.x);
        }
        else if (hole.Normal == Vector3.forward || hole.Normal == Vector3.back)
        {
            subtractor.transform.position = mesh.transform.position + meshBounds.center + new Vector3(hole.Position.x, hole.Position.y, 0);
            subtractor.transform.localScale = new Vector3(hole.Size.x, hole.Size.y, meshBounds.size.z);
        }


        return subtractor;
    }
}
