using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RectangleMeshGenerator
{
    private Material _material;
    private float _height;
    public RectangleMeshGenerator(Material material, float height)
    {
        _material = material;
        _height = height;
    }
    public virtual GameObject GenerateMesh(Rectangle rect)
    {
        GameObject mesh = new GameObject();
        mesh.transform.position = rect.p1;
        mesh.AddComponent<RectangleMesh>().Init(rect);
        Mesh m = new Mesh();
        m = GetCubeMesh(rect);


        if (rect.p1 != rect.p2)
        {
            mesh.AddComponent<MeshFilter>().mesh = m;
            mesh.AddComponent<MeshRenderer>().material = _material;
            mesh.AddComponent<MeshCollider>();
        }

        return mesh;
    }


    Mesh GetCubeMesh(Rectangle rect)
    {
        var points = rect.GetPoints();

        Vector3 height = new Vector3(0, _height, 0);

        Vector3[] c = new Vector3[8];

        c[0] = points.p1;
        c[1] = points.p2;
        c[2] = points.p3;
        c[3] = points.p4;

        c[4] = points.p1 + height;
        c[5] = points.p2 + height;
        c[6] = points.p3 + height;
        c[7] = points.p4 + height;

        Vector3[] vertices = new Vector3[24]
        {
            //points.p1, points.p2, points.p3, points.p4,

            //points.p1,points.p1+height,points.p2+height,points.p2,

            //points.p2,points.p2+ height, points.p3 + height,points.p3,

            //points.p3,points.p3+height,points.p4 + height,points.p4,

            //points.p4,points.p4+height,points.p1 + height,points.p1,

            //points.p1 + height, points.p2+ height, points.p3+ height, points.p4+ height,

            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top

        };

        Vector3[] normals = new Vector3[24]
        {
            Vector3.down,Vector3.down,Vector3.down,Vector3.down,
            Vector3.left,Vector3.left,Vector3.left,Vector3.left,
            Vector3.forward,Vector3.forward,Vector3.forward,Vector3.forward,
            Vector3.back,Vector3.back,Vector3.back,Vector3.back,
            Vector3.right,Vector3.right,Vector3.right,Vector3.right,
            Vector3.up,Vector3.up,Vector3.up,Vector3.up,
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] - rect.p1;
        }

        int[] triangles = new int[36];


        if ((rect.p1.x < rect.p2.x && rect.p1.z > rect.p2.z) || (rect.p1.x > rect.p2.x && rect.p1.z < rect.p2.z))
        {
            for (int i = 0, y = 0; i < triangles.Length; i += 6, y += 4)
            {
                triangles[i] = y + 2;
                triangles[i + 1] = y + 1;
                triangles[i + 2] = y;

                triangles[i + 3] = y;
                triangles[i + 4] = y + 3;
                triangles[i + 5] = y + 2;
            }
        }
        else
        {
            for (int i = 0, y = 0; i < triangles.Length; i += 6, y += 4)
            {
                triangles[i] = y;
                triangles[i + 1] = y + 1;
                triangles[i + 2] = y + 2;

                triangles[i + 3] = y + 2;
                triangles[i + 4] = y + 3;
                triangles[i + 5] = y;
            }
        }



        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = normals;
        return m;
    }


}
