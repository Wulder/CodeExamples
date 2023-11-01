using Parabox.CSG;
using System.Collections;
using System.Collections.Generic;
using TriLibCore.Dae.Schema;
using UnityEngine;

public class RectangleMeshCreator : MonoBehaviour
{
    [SerializeField] private RectanglesPlan _plan;
    [SerializeField] private Material _material;
    [SerializeField] private float _height;

    [SerializeField] private float _holeCutterThickness = 0.01f;
    private Dictionary<Rectangle, GameObject> _rectangleMeshes = new Dictionary<Rectangle, GameObject>();


    private RectangleMeshGenerator _generator;

    private GameObject _holeCutter;
    private RectangleHole _hole;
    public RectangleHole Hole => _hole;
    private RectangleMesh _holeMesh;

    private void Awake()
    {
        _generator = new RectangleMeshGenerator(_material, _height);
    }

    private void OnEnable()
    {
        _plan.OnAddRectangle += AddRectangleMesh;
        _plan.OnUpdateRectangle += RebuildRectangle;
        _plan.OnDeleteRectangle += RemoveRectangle;
        
    }

    private void OnDisable()
    {
        _plan.OnAddRectangle -= AddRectangleMesh;
        _plan.OnUpdateRectangle -= RebuildRectangle;
        _plan.OnDeleteRectangle -= RemoveRectangle;
    }

    private void AddRectangleMesh(Rectangle rect)
    {
        
        _rectangleMeshes[rect] = _generator.GenerateMesh(rect);
        _rectangleMeshes[rect].gameObject.layer = LayerMask.NameToLayer("Wall");

        _rectangleMeshes[rect].transform.SetParent(transform, false);
        
        foreach(var h in rect.Holes)
        {
            CutHole(rect, h);
        }

    }

    private void RemoveRectangle(Rectangle rect)
    {
        if (!_rectangleMeshes.ContainsKey(rect)) return;
        Destroy(_rectangleMeshes[rect]);
        _rectangleMeshes.Remove(rect);

    }

    private void RebuildRectangle(Rectangle rect)
    {
        RemoveRectangle(rect);
        AddRectangleMesh(rect);
    }

    private void CreateHoleCutter(Rectangle rect, RectangleHole hole)
    {
        _hole = hole;
        _holeMesh = _rectangleMeshes[rect].GetComponent<RectangleMesh>();

        if(_holeCutter) Destroy(_holeCutter);
        

        _holeCutter = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var meshBounds = _holeMesh.GetComponent<MeshFilter>().mesh.bounds;
        _holeCutter.transform.position = _holeMesh.transform.position + meshBounds.center;

        if (hole.Normal == Vector3.right || hole.Normal == Vector3.left) _holeCutter.transform.localScale = new Vector3(meshBounds.size.x * (1+_holeCutterThickness), hole.Size.y, hole.Size.x);
        if (hole.Normal == Vector3.forward || hole.Normal == Vector3.back) _holeCutter.transform.localScale = new Vector3(hole.Size.x, hole.Size.y, meshBounds.size.z * (1 + _holeCutterThickness));


    }

    void UpdateHoleCutter()
    { 
        var meshBounds = _holeMesh.GetComponent<MeshFilter>().mesh.bounds;
       

        if (_hole.Normal == Vector3.right || _hole.Normal == Vector3.left)
        {
            _holeCutter.transform.position = _holeMesh.transform.position + meshBounds.center + new Vector3(0,_hole.Position.y, _hole.Position.x);
            _holeCutter.transform.localScale = new Vector3(meshBounds.size.x * (1 + _holeCutterThickness), _hole.Size.y, _hole.Size.x);
        }
        else if(_hole.Normal == Vector3.forward || _hole.Normal == Vector3.back)
        {
            _holeCutter.transform.position = _holeMesh.transform.position + meshBounds.center + new Vector3(_hole.Position.x, _hole.Position.y,0);
            _holeCutter.transform.localScale = new Vector3(_hole.Size.x, _hole.Size.y, meshBounds.size.z * (1 + _holeCutterThickness));
        }
    }

    public void UpdateHoleSize(Vector2 size)
    {
        _hole.Size = size;
        UpdateHoleCutter();
    }

    public void UpdateHolePos(Vector2 pos)
    {
        _hole.Position = pos;
        UpdateHoleCutter();
    }
    public void PerformHole()
    {
        Model result = CSG.Subtract(_holeMesh.gameObject, _holeCutter);

        Vector3[] newVerts = result.mesh.vertices;
        for(int i = 0; i < newVerts.Length; i++)
        {
            newVerts[i] -= _holeMesh.transform.position;
        }

        result.mesh.vertices = newVerts;
        Mesh newMesh = new Mesh();
        newMesh.vertices = newVerts;
        newMesh.triangles = result.mesh.triangles;
        newMesh.normals = result.mesh.normals;

        _holeMesh.GetComponent<MeshFilter>().mesh = newMesh;
        //_holeMesh.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        _holeMesh.GetComponent<MeshCollider>().sharedMesh = newMesh;

        

        Destroy(_holeCutter);
        _holeMesh = null;
    }

    public void CutHole(Rectangle rect, RectangleHole hole)
    {
        
        CreateHoleCutter(rect,hole);
        UpdateHoleCutter();
        PerformHole();
    }
}
