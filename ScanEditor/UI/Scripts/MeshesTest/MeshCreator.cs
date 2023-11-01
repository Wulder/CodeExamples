using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC
{
    public class MeshCreator : MonoBehaviour
    {
        GameObject newObj;

        //public void CreateEmptyObject()
        //{
        //    newObj = new GameObject("Simple GameObject");
        //    newObj.AddComponent<MeshFilter>();
        //    newObj.AddComponent<MeshRenderer>();
        //    newObj.AddComponent<BoxCollider>();
        //}

        //private void CreateSimpleMesh()
        //{
        //    CreateEmptyObject();

        //    Mesh mesh = new Mesh();
        //    mesh.name = "Simple mesh";

        //    List<Vector3> normals = new List<Vector3>();
        //    GetNormals(ref normals);
        //    List<Vector3> vertexes = new List<Vector3>();
        //    GetVertexes(ref vertexes, normals);

        //    mesh.normals = normals.ToArray();

        //    mesh.vertices = new Vector3[]{
        //    Vector3.zero, Vector3.right, Vector3.up};

        //    mesh.triangles = new int[] { 0, 2, 1 };
        //    mesh.normals = new Vector3[] {
        //    Vector3.forward, Vector3.forward, Vector3.forward};

        //    newObj.GetComponent<MeshFilter>().mesh = mesh;
        //}

        //private void GetNormals(ref List<Vector3> planes)
        //{
        //    Vector3 normal = Vector3.zero;

        //    for (int i = 0; i < meshes[0].mesh.normals.Length; i++)
        //    {
        //        if (normal == Vector3.zero)
        //        {
        //            normal = meshes[0].mesh.normals[i];
        //            planes.Add(normal);
        //            continue;
        //        }

        //        if (meshes[0].mesh.normals[i] == normal)
        //        {
        //            planes.Add(meshes[0].mesh.normals[i]);
        //        }
        //    }
        //}

        //private void GetVertexes(ref List<Vector3> vertexes, List<Vector3> normals)
        //{

        //}
    }
}

