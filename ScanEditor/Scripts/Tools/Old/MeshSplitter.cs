using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public class MeshSplitter : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) { SplitTriangles(); }
    }

    [ContextMenu("Split")]
    private void SplitTriangles()
    {
        Mesh m = MeshSelector.SelectedMesh.GetComponent<MeshFilter>().mesh;
        List<Mesh> subMeshes = new List<Mesh>();
        List<Vector3> verts = m.vertices.ToList();

        int[] tris = m.triangles;
        List<TriangleIndicies> freeTris = new List<TriangleIndicies>();

        for (int i = 0; i < tris.Length; i += 3)
        {
            freeTris.Add(new TriangleIndicies(tris[i], tris[i + 1], tris[i + 2]));
        }

        Dictionary<int, List<TriangleIndicies>> trisGroups = new Dictionary<int, List<TriangleIndicies>>();

        Dictionary<Vector3, HashSet<Mesh>> meshGroups = new Dictionary<Vector3, HashSet<Mesh>>();

        foreach (var t in freeTris)
        {
            if (!trisGroups.ContainsKey(t.p0))
            {
                trisGroups[t.p0] = new List<TriangleIndicies>();
            }
            trisGroups[t.p0].Add(t);

            if (!trisGroups.ContainsKey(t.p1))
            {
                trisGroups[t.p1] = new List<TriangleIndicies>();
            }
            trisGroups[t.p1].Add(t);

            if (!trisGroups.ContainsKey(t.p2))
            {
                trisGroups[t.p2] = new List<TriangleIndicies>();
            }
            trisGroups[t.p2].Add(t);
        }

        while (freeTris.Count > 0)
        {
            HashSet<TriangleIndicies> substarctedTris = new HashSet<TriangleIndicies>();
            substarctedTris.Add(freeTris.First());
            freeTris.Remove(freeTris.First());
            Mesh subMesh = new Mesh();
            List<TriangleIndicies> selected = new List<TriangleIndicies>();

            do
            {

                selected = SelectTriangles(ref trisGroups, substarctedTris.ToArray());

                foreach (var t in selected)
                {
                    if (!meshGroups.ContainsKey(verts[t.p0]))
                        meshGroups[verts[t.p0]] = new HashSet<Mesh>();

                    if (!meshGroups.ContainsKey(verts[t.p1]))
                        meshGroups[verts[t.p1]] = new HashSet<Mesh>();

                    if (!meshGroups.ContainsKey(verts[t.p2]))
                        meshGroups[verts[t.p2]] = new HashSet<Mesh>();

                    meshGroups[verts[t.p0]].Add(subMesh);
                    meshGroups[verts[t.p1]].Add(subMesh);
                    meshGroups[verts[t.p2]].Add(subMesh);

                    freeTris.Remove(t);
                    // if (!substarctedTris.Exists(tris => tris.p0 == t.p0 || tris.p1 == t.p1 || tris.p2 == t.p2))
                    substarctedTris.Add(t);
                }
            }
            while (selected.Count > 0);


            //creating mesh and add it to submeshes

            subMesh.vertices = verts.ToArray();
            subMesh.triangles = TrisToIntArray(substarctedTris.ToList());
            subMesh.uv = m.uv;
            subMeshes.Add(subMesh);

        }


        HashSet<Mesh> substractedMeshes = new HashSet<Mesh>();


        while (subMeshes.Count > 0)
        {
            HashSet<Mesh> separatedMeshes = new HashSet<Mesh>();

            HashSet<Mesh> selected = new HashSet<Mesh>();
            selected.Add(subMeshes.First());
            separatedMeshes.Add(subMeshes.First());

            while (selected.Count > 0)
            {
                selected = SelectMeshes(selected, meshGroups);

                foreach (Mesh mesh in selected)
                {
                    separatedMeshes.Add(mesh);

                }
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.vertices = verts.ToArray();
            combinedMesh.uv = m.uv;
            List<int> combinedTris = new List<int>();
            foreach (var separatedMesh in separatedMeshes)
            {
                subMeshes.Remove(separatedMesh);
                foreach (int t in separatedMesh.triangles)
                    combinedTris.Add(t);
            }

            substractedMeshes.Add(combinedMesh);

            List<TriangleIndicies> outTris = new List<TriangleIndicies>();
            List<Vector3> outVerts = new List<Vector3>();
            List<Vector2> outUv = new List<Vector2>();

            List<TriangleIndicies> trisList = ListTrisFromIntArray(combinedTris.ToArray());

            GetLocalVerticesAndTris(trisList, combinedMesh.vertices, combinedMesh.uv, out outTris, out outVerts, out outUv);

            combinedMesh.vertices = outVerts.ToArray();
            combinedMesh.uv = outUv.ToArray();
            combinedMesh.triangles = TrisToIntArray(outTris);

            combinedMesh.RecalculateBounds();

            Vector3 offset = combinedMesh.bounds.center;

            for (int i = 0; i < outVerts.Count; i++)
                outVerts[i] -= offset;


            combinedMesh.vertices = outVerts.ToArray();
            
            ObjectCreator.CreateObjectFromMesh(combinedMesh,   ApplicationController.Instance.GetComponent<MeshRenderer>().material, combinedMesh.bounds.center, true, "subObject", new CreationParameters() { Highlighting = true });

        }

        Debug.Log($"Count of separated meshes: {substractedMeshes.Count}");
        Destroy(MeshSelector.SelectedMesh);
    }


    HashSet<Mesh> SelectMeshes(HashSet<Mesh> meshes, Dictionary<Vector3, HashSet<Mesh>> meshGroups)
    {
        HashSet<Mesh> result = new HashSet<Mesh>();

        foreach (var m in meshes)
        {
            foreach (var t in m.triangles)
            {
                if (meshGroups.ContainsKey(m.vertices[t]))
                {
                    foreach (Mesh mesh in meshGroups[m.vertices[t]])
                    {
                        result.Add(mesh);
                    }

                    meshGroups.Remove(m.vertices[t]);
                }
            }
        }

        return result;
    }
    bool CheckTriangleConnection(TriangleIndicies t, List<TriangleIndicies> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (t.p0 == connections[i].p0 || t.p0 == connections[i].p1 || t.p0 == connections[i].p2 ||
                t.p1 == connections[i].p0 || t.p1 == connections[i].p1 || t.p1 == connections[i].p2 ||
                t.p2 == connections[i].p0 || t.p2 == connections[i].p1 || t.p2 == connections[i].p2)
            {
                return true;
            }
        }
        return false;


    }

    List<TriangleIndicies> SelectTriangles(ref Dictionary<int, List<TriangleIndicies>> sortedTris, TriangleIndicies[] connections)
    {
        HashSet<TriangleIndicies> result = new HashSet<TriangleIndicies>();

        for (int i = 0; i < connections.Length; i++)
        {
            List<TriangleIndicies> p0Selection = new List<TriangleIndicies>();
            List<TriangleIndicies> p1Selection = new List<TriangleIndicies>();
            List<TriangleIndicies> p2Selection = new List<TriangleIndicies>();
            foreach (var t in sortedTris[connections[i].p0])
            {
                if (!result.Contains(t))
                {
                    p0Selection.Add(t);


                }

            }
            foreach (var t in p0Selection)
            {
                sortedTris[connections[i].p0].Remove(t);
            }
            foreach (var t in sortedTris[connections[i].p1])
            {
                if (!result.Contains(t))
                {
                    p1Selection.Add(t);
                }

            }
            foreach (var t in p1Selection)
            {
                sortedTris[connections[i].p1].Remove(t);
            }
            foreach (var t in sortedTris[connections[i].p2])
            {
                if (!result.Contains(t))
                {
                    p2Selection.Add(t);
                }

            }
            foreach (var t in p2Selection)
            {
                sortedTris[connections[i].p2].Remove(t);
            }

            foreach (var t in p0Selection)
                result.Add(t);

            foreach (var t in p1Selection)
                result.Add(t);

            foreach (var t in p2Selection)
                result.Add(t);


        }

        return result.ToList();
    }

    public static int[] TrisToIntArray(List<TriangleIndicies> tris)
    {
        int[] resultTris = new int[tris.Count * 3];

        for (int i = 0, index = 0; i < resultTris.Length; i += 3, index++)
        {
            resultTris[i] = tris[index].p0;
            resultTris[i + 1] = tris[index].p1;
            resultTris[i + 2] = tris[index].p2;
        }

        return resultTris;
    }

    public static List<TriangleIndicies> ListTrisFromIntArray(int[] tris)
    {
        List<TriangleIndicies> result = new List<TriangleIndicies>();


        for (int i = 0; i < tris.Length; i += 3)
        {
            result.Add(new TriangleIndicies(tris[i], tris[i + 1], tris[i + 2]));
        }

        return result;
    }

    void GetLocalVerticesAndTris(List<TriangleIndicies> tris, Vector3[] srcVertices, Vector2[] srcUv, out List<TriangleIndicies> outResultTris, out List<Vector3> outResultVertices, out List<Vector2> outResultUv)
    {
        List<TriangleIndicies> resultTris = tris;
        List<Vector3> resultVertices = new List<Vector3>();
        List<Vector2> resultUv = new List<Vector2>();

        Dictionary<int, int> sortedVerticesCorrespondences = new Dictionary<int, int>();
        int sortedVertsIterator = 0;
        for (int i = 0; i < tris.Count; i++)
        {
            if (!sortedVerticesCorrespondences.ContainsKey(tris[i].p0))
            {
                sortedVerticesCorrespondences[tris[i].p0] = sortedVertsIterator++;
                resultVertices.Add(srcVertices[tris[i].p0]);
                resultUv.Add(srcUv[tris[i].p0]);
            }

            if (!sortedVerticesCorrespondences.ContainsKey(tris[i].p1))
            {
                sortedVerticesCorrespondences[tris[i].p1] = sortedVertsIterator++;
                resultVertices.Add(srcVertices[tris[i].p1]);
                resultUv.Add(srcUv[tris[i].p1]);
            }

            if (!sortedVerticesCorrespondences.ContainsKey(tris[i].p2))
            {
                sortedVerticesCorrespondences[tris[i].p2] = sortedVertsIterator++;
                resultVertices.Add(srcVertices[tris[i].p2]);
                resultUv.Add(srcUv[tris[i].p2]);
            }
        }

        for (int i = 0; i < tris.Count; i++)
        {
            resultTris[i] = new TriangleIndicies(sortedVerticesCorrespondences[tris[i].p0], sortedVerticesCorrespondences[tris[i].p1], sortedVerticesCorrespondences[tris[i].p2]);
        }

        outResultTris = resultTris;
        outResultVertices = resultVertices;
        outResultUv = resultUv;
    }


}

[Serializable]
public struct TriangleIndicies
{
    public int p0, p1, p2;

    public TriangleIndicies(int p0, int p1, int p2)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
    }


}