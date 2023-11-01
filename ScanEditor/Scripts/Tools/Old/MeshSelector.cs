using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MeshSelector : OldTool
{

    public static event Action<GameObject> OnSelectedMesh;

    private static GameObject Mesh;
    private static Collider _collider { get { return Mesh.GetComponent<Collider>(); } set { } }
    private static MeshFilter _filter;
    private static MeshRenderer _renderer;
    public static GameObject SelectedMesh => Mesh;
    public static Collider Collider => _collider;
    public static MeshFilter Filter => _filter;
    public static MeshRenderer Renderer => _renderer;

    private MeshInfo _meshInfo;

    [SerializeField] private MeshRoot _meshRoot;
    [SerializeField] private GameObject _defaultMesh;

    private void OnEnable()
    {
        _meshRoot.OnNewMesh += UpdateSelectedmesh;
    }

    private void OnDisable()
    {
        _meshRoot.OnNewMesh -= UpdateSelectedmesh;
    }
    private void Awake()
    {
        _meshInfo = new MeshInfo();
        UpdateSelectedmesh(_defaultMesh);
    }
    // Update is called once per frame
    

    void UpdateSelectedmesh(GameObject mesh)
    {
        Mesh = mesh;
        _filter = mesh.GetComponent<MeshFilter>();
        _collider = mesh.GetComponent<MeshCollider>();
        _renderer = mesh.GetComponent<MeshRenderer>();

        Mesh m = _filter.mesh;

        _meshInfo.Verts = m.vertexCount;
        _meshInfo.Triangles = m.triangles.Length;
        _meshInfo.UV = m.uv.Length;
        _meshInfo.SubMeshes = m.subMeshCount;
        Debug.Log($"Selected new mesh {mesh.name}");
    }

    private void OnGUI()
    {
        return;
        Rect rect = new Rect(25, 25, 250, 500);

        if (Mesh)
        {
            GUI.Label(rect, $"Name: {SelectedMesh.name}");
            rect.y += 20;
            GUI.Label(rect, $"Vertices: {_meshInfo.Verts}");
            rect.y += 20;
            GUI.Label(rect, $"Triangles: {_meshInfo.Triangles} ({_meshInfo.Triangles / 3} tris.)");
            rect.y += 20;
            GUI.Label(rect, $"UV: {_meshInfo.UV}");
            rect.y += 20;
            GUI.Label(rect, $"Sub meshes: {_meshInfo.SubMeshes}");
            rect.y += 20;

            if(_collider != null)
            {
                Bounds bounds = _collider.bounds;
                GUI.Label(rect, $"----------------------\nMesh Bounds:\n Center: {bounds.center}\n MinY/MaxY: {bounds.min.y}/{bounds.max.y}\n Size: {bounds.size}\n" +
                    $"LocalCenter: {bounds.center - MeshSelector.SelectedMesh.transform.position}\nLocal MinY/MaxY {bounds.min.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y}/{bounds.max.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y}\n----------------------");
                rect.y += 20;
            }
            
        }

    }

    struct MeshInfo
    {
        public int Verts;
        public int Triangles;
        public int UV;
        public int SubMeshes;
    }
}


