using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMesh : MonoBehaviour
{
    [SerializeField] private GameObject mesh;

    int totalMeshes = 0;
    int totalVertices = 0;
    int totalTris = 0;

    Dictionary<int, int> topList = new Dictionary<int, int>();

    MeshFilter[] meshes;
    int[] trg;
    GameObject newObj;

    private void Start()
    {
        meshes = mesh.GetComponentsInChildren<MeshFilter>();

        for (int i = 0, lenght = meshes.Length; i < lenght; i++)
        {
            int verts = meshes[i].sharedMesh.vertexCount;
            totalVertices += verts;
            if (meshes[i].sharedMesh.GetTopology(0) == MeshTopology.Triangles)
            {
                totalTris += meshes[i].sharedMesh.triangles.Length / 3;
                AddTriangles(ref trg, meshes[i].sharedMesh.triangles);
            }
            else
            {
                Debug.LogWarning("Объект не треугольной топологии!");
                break;
            }
            totalMeshes++;
            topList.Add(i, verts);
        }

        Debug.Log("Total object tris = " + totalTris);

        //CreateSimpleMesh();
    }

    private void AddTriangles(ref int[] target, int[] tris)
    {
        if (target == null || target.Length == 0)
        {
            target = tris;
        }
        else
        {
            List<int> tmp = new List<int>();
            tmp.AddRange(target);
            tmp.AddRange(tris);
            target = tmp.ToArray();
            tmp.Clear();
        }
    }
}
