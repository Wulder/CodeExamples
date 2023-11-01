using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class MeshSubtractorTool : Tool
{
    [SerializeField] private CubeSubtract _subtract;
    [SerializeField] private Transform _substractPivot;
    [SerializeField] private float _maxFillingVerticesDistance = .4f;
    [SerializeField] private Material _fillingMaterial;
    [SerializeField] private bool _fillHoles;
    [SerializeField] private bool _debugSubstract;

    private SubtractorCubeGizmo _subtractCubeGizmo;

    private float _substractTimer;
    private List<Triangle> _overlappedTriangles = new List<Triangle>();
    private List<HashSet<Vector3>> _contours = new List<HashSet<Vector3>>();

    private SubtractorUI _ui;

    public MeshSubtractorTool()
    {
        _subtract.size = Vector3.one;
    }

    
    public override void ToolInput()
    {
        _subtract.center = _substractPivot.position;
        _subtract.rotation = _substractPivot.rotation;
        _subtract.size = _subtractCubeGizmo.GetScaleFactor();
        _subtractCubeGizmo?.SetSize(_subtract.size);
    }

    public void Substract()
    {
        _overlappedTriangles.Clear();
        _contours.Clear();
        HashSet<Vector3> selectedEdgeVertices = new HashSet<Vector3>();

        Dictionary<Vector3, HashSet<EdgeTriangle>> _sortedEdgedTriangles = new Dictionary<Vector3, HashSet<EdgeTriangle>>();

        #region Substract
        MeshFilter mFilter = ApplicationController.Instance.MainMesh.GetComponent<MeshFilter>();
        Mesh m = mFilter.mesh;

        Vector3[] vertices = m.vertices;
        int[] triangles = m.triangles;
        Vector2[] uv = m.uv;

        CuttedArray<int> resultTriangles = new CuttedArray<int>(triangles.Length);

        List<Vertice> substractedvertices = new List<Vertice>();
        List<int> substractedTriangles = new List<int>();
        List<Vector2> substractedUv = new List<Vector2>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index0 = triangles[i], index1 = triangles[i + 1], index2 = triangles[i + 2];
            bool p0IsIncluded = CheckPointInsideCube((ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index0] + ApplicationController.Instance.MainMesh.transform.position));
            bool p1IsIncluded = CheckPointInsideCube((ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index1] + ApplicationController.Instance.MainMesh.transform.position));
            bool p2IsIncluded = CheckPointInsideCube((ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index2] + ApplicationController.Instance.MainMesh.transform.position));
            if (p0IsIncluded || p1IsIncluded || p2IsIncluded)
            {

                Vector3 v0 = (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index0] + ApplicationController.Instance.MainMesh.transform.position) - _subtract.center;
                Vector3 v1 = (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index1] + ApplicationController.Instance.MainMesh.transform.position) - _subtract.center;
                Vector3 v2 = (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index2] + ApplicationController.Instance.MainMesh.transform.position) - _subtract.center;

                if (!p0IsIncluded) selectedEdgeVertices.Add(v0);
                if (!p1IsIncluded) selectedEdgeVertices.Add(v1);
                if (!p2IsIncluded) selectedEdgeVertices.Add(v2);

                if (!p0IsIncluded || !p1IsIncluded || !p2IsIncluded)
                {

                    if (!_sortedEdgedTriangles.ContainsKey(v0))
                        _sortedEdgedTriangles[v0] = new HashSet<EdgeTriangle>();
                    if (!_sortedEdgedTriangles.ContainsKey(v1))
                        _sortedEdgedTriangles[v1] = new HashSet<EdgeTriangle>();
                    if (!_sortedEdgedTriangles.ContainsKey(v2))
                        _sortedEdgedTriangles[v2] = new HashSet<EdgeTriangle>();

                    _sortedEdgedTriangles[v0].Add(new EdgeTriangle(v0, v1, v2, p0IsIncluded, p1IsIncluded, p2IsIncluded));
                    _sortedEdgedTriangles[v1].Add(new EdgeTriangle(v0, v1, v2, p0IsIncluded, p1IsIncluded, p2IsIncluded));
                    _sortedEdgedTriangles[v2].Add(new EdgeTriangle(v0, v1, v2, p0IsIncluded, p1IsIncluded, p2IsIncluded));

                }





                int vIndex0, vIndex1, vIndex2;
                if (AddVertice(ref substractedvertices, index0, (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index0]) - _subtract.center + ApplicationController.Instance.MainMesh.transform.position, out vIndex0))
                {
                    substractedUv.Add(uv[index0]);
                }
                else
                {
                    substractedUv[vIndex0] = uv[index0];
                }

                if (AddVertice(ref substractedvertices, index1, (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index1]) - _subtract.center + ApplicationController.Instance.MainMesh.transform.position, out vIndex1))
                {
                    substractedUv.Add(uv[index1]);
                }
                else
                {
                    substractedUv[vIndex1] = uv[index1];
                }

                if (AddVertice(ref substractedvertices, index2, (ApplicationController.Instance.MainMesh.transform.rotation) * (vertices[index2]) - _subtract.center + ApplicationController.Instance.MainMesh.transform.position, out vIndex2))
                {
                    substractedUv.Add(uv[index2]);
                }
                else
                {
                    substractedUv[vIndex2] = uv[index2];
                }

                AddTris(ref substractedTriangles, vIndex0, vIndex1, vIndex2);

            }
            else
            {
                resultTriangles.AddElement(triangles[i]);
                resultTriangles.AddElement(triangles[i + 1]);
                resultTriangles.AddElement(triangles[i + 2]);
            }
        }

        mFilter.mesh.triangles = resultTriangles.GetArray();
        GameObject.Destroy(mFilter.GetComponent<MeshCollider>());
        mFilter.sharedMesh.RecalculateNormals();
        mFilter.AddComponent<MeshCollider>();

        Vector3[] verticeArray = new Vector3[substractedvertices.Count];
        int vIndex = 0;
        foreach (var v in substractedvertices) { verticeArray[vIndex] = v.Position; vIndex++; }

        Mesh mesh = new Mesh();
        mesh.vertices = verticeArray;
        mesh.triangles = substractedTriangles.ToArray();
        mesh.uv = substractedUv.ToArray();
        ObjectCreator.CreateObjectFromMesh(mesh, ApplicationController.Instance.MainMesh.GetComponent<MeshRenderer>().material, _subtract.center, true, "subObject", new CreationParameters() { Highlighting = true });
        #endregion

        //filling holes after substract
        if (!_fillHoles) return;
        #region Filling Holes


        HashSet<Vector3> oldKeys = new HashSet<Vector3>();

        while (selectedEdgeVertices.Count > 0)
        {
            _contours.Add(new HashSet<Vector3>());

            Vector3 keyVertex = selectedEdgeVertices.First();
            selectedEdgeVertices.Remove(keyVertex);
            _contours.Last().Add(keyVertex);
            oldKeys.Add(keyVertex);
            for (int i = 0; i < 500; i++)
            {

                HashSet<EdgeTriangle> selected = _sortedEdgedTriangles[keyVertex];
                _sortedEdgedTriangles.Remove(keyVertex);
                bool endContour = false;
                foreach (var t in selected)
                {
                    if (!t.p0Included && !oldKeys.Contains(t.p0))
                    {
                        endContour = false;
                        _contours.Last().Add(t.p0);
                        selectedEdgeVertices.Remove(t.p0);
                        keyVertex = t.p0;
                        oldKeys.Add(keyVertex);
                        break;
                    }
                    if (!t.p1Included && !oldKeys.Contains(t.p1))
                    {
                        endContour = false;
                        _contours.Last().Add(t.p1);
                        selectedEdgeVertices.Remove(t.p1);
                        keyVertex = t.p1;
                        oldKeys.Add(keyVertex);
                        break;
                    }
                    if (!t.p2Included && !oldKeys.Contains(t.p2))
                    {
                        endContour = false;
                        _contours.Last().Add(t.p2);
                        selectedEdgeVertices.Remove(t.p2);
                        keyVertex = t.p2;
                        oldKeys.Add(keyVertex);
                        break;
                    }

                    endContour = true;
                }

                if (endContour) break;

            }

            if (_contours.Last().Count < 3) { _contours.Remove(_contours.Last()); }
        }

        HoleFiller filler = new HoleFiller();
        //Mesh filling = filler.Fill(_contours[0].ToList());
        //var obj = ObjectCreator.CreateObjectFromMesh(filling, _substract.center);
        //obj.GetComponent<MeshRenderer>().material = _fillingMaterial;

        foreach (var c in _contours)
        {
            Mesh filling = filler.Fill(c.ToList());


            var obj = ObjectCreator.CreateObjectFromMesh(filling, ApplicationController.Instance.MainMesh.GetComponent<MeshRenderer>().material, _subtract.center, false, "filledHole", new CreationParameters() { Highlighting = true });
            obj.transform.SetParent(ApplicationController.Instance.MainMesh.transform);
            obj.GetComponent<MeshRenderer>().material = ApplicationController.Instance.AppConfig.FillHolesMaterial;
        }

        #endregion

    }

    void AddTris(ref List<int> tris, int p0, int p1, int p2)
    {
        tris.Add(p0); tris.Add(p1); tris.Add(p2);
    }

    bool AddVertice(ref List<Vertice> vertices, int vIndex, Vector3 pos, out int index)
    {
        if (vertices.Exists(v => v.Index == vIndex))
        {
            index = vertices.IndexOf(vertices.Find(v => v.Index == vIndex));
            return false;
        }
        else
        {
            vertices.Add(new Vertice(vIndex, pos));
            index = vertices.Count - 1;
            return true;
        }
    }

    private bool CheckPointInsideCube(Vector3 pointPos)
    {
        // Get the position, rotation, and scale of the cube
        Vector3 cubePosition = _substractPivot.transform.position;
        Quaternion cubeRotation = _substractPivot.transform.rotation;
        Vector3 cubeScale = _subtract.size;

        // Convert the point to the local coordinate system of the cube
        Vector3 localPoint = pointPos - cubePosition;

        // Apply the inverse rotation to the point
        Quaternion inverseRotation = Quaternion.Inverse(cubeRotation);
        Vector3 rotatedPoint = inverseRotation * localPoint;


        // Check if the point is inside the cube
        bool insideBounds = Mathf.Abs(rotatedPoint.x) <= cubeScale.x / 2f &&
        Mathf.Abs(rotatedPoint.y) <= cubeScale.y / 2f &&
        Mathf.Abs(rotatedPoint.z) <= cubeScale.z / 2f;

        if (insideBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void DrawGUI()
    {
        //Rect rect = new Rect(25, 25, 100, 50);
        //if (GUI.Button(rect, "Substract"))
        //{
        //    Substract();
        //}
    }

    public override void DrawGizmos()
    {
        //Debug.Log(_subtract.size);
        //Gizmos.matrix = Matrix4x4.TRS(_substractPivot.position, _substractPivot.rotation, Vector3.one);
        //if (_debugSubstract)
        //{
        //    Gizmos.color = new Color(0, 1, 0, 0.5f);


        //    Gizmos.DrawCube(Vector3.zero, _subtract.size);
        //}
        //int listNum = 0;
        //foreach (var list in _contours)
        //{

        //    int pointNum = 0;
        //    Vector3 center = Vector3.zero;
        //    foreach (var v in list)
        //    {
        //        center += v;
        //        Gizmos.DrawCube(v, Vector3.one * 0.005f);
        //        Handles.Label(v + _subtract.center, pointNum.ToString());
        //        pointNum++;
        //    }

        //    Handles.Label(center / list.Count + _subtract.center, (listNum++).ToString());
        //}
    }

    public override void Enable()
    {
        GameObject gm = Resources.Load("ToolsUI/SubtractorUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(gm).GetComponent<SubtractorUI>();
        _ui.Init(this);

        gm = Resources.Load("Tools/SubtractorObject", typeof(GameObject)) as GameObject;
        _substractPivot = GameObject.Instantiate(gm).transform;
        _subtractCubeGizmo = _substractPivot.GetComponent<SubtractorCubeGizmo>();
    }

    public override void Disable()
    {
        GameObject.Destroy(_ui.gameObject);
       GameObject.Destroy( _substractPivot.gameObject);
       
    }

    public void SetXSize(string size)
    {
        float fsize = float.Parse(size);
        _subtract.size.x = fsize;
    }
    public void SetYSize(string size)
    {
        float fsize = float.Parse(size);
        _subtract.size.y = fsize;
    }
    public void SetZSize(string size)
    {
        float fsize = float.Parse(size);
        _subtract.size.z = fsize;
    }
    public void SetFillHoles(bool val)
    {
        _fillHoles = val;
    }
}

[Serializable]
struct CubeSubtract
{
    public Vector3 center;
    public Vector3 size;
    public Quaternion rotation;

}

struct CuttedArray<T>
{
    private T[] _array;
    private int _index;

    public CuttedArray(int size)
    {
        _array = new T[size];
        _index = 0;
    }

    public void AddElement(T element)
    {
        _array[_index++] = element;
    }

    public T[] GetArray()
    {
        return _array[.._index];
    }
}

struct Vertice
{
    public int Index;
    public Vector3 Position;

    public Vertice(int index, Vector3 pos)
    {
        Index = index;
        Position = pos;
    }
}

struct EdgeTriangle
{
    public Vector3 p0, p1, p2;
    public bool p0Included, p1Included, p2Included;

    public EdgeTriangle(Vector3 point0, Vector3 point1, Vector3 point2, bool included0, bool included1, bool included2)
    {
        p0 = point0;
        p1 = point1;
        p2 = point2;
        p0Included = included0;
        p1Included = included1;
        p2Included = included2;
    }
}

