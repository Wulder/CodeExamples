using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class MeshInfo : MonoBehaviour
{
#if UNITY_EDITOR
    private MeshFilter _currentMeshFilter;
    private int _verticiesCount, _trianglesCount, _uvCount;
    private void OnEnable()
    {
        Selection.selectionChanged += UpdateMeshInfo;
    }

    private void OnDisable()
    {

        Selection.selectionChanged -= UpdateMeshInfo;
    }

    void UpdateMeshInfo()
    {
      _currentMeshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
        if (_currentMeshFilter == null) return;

        _verticiesCount = _currentMeshFilter.mesh.vertexCount;
        _trianglesCount = _currentMeshFilter.mesh.triangles.Length;
        _uvCount = _currentMeshFilter.mesh.uv.Length;
    }
    private void OnGUI()
    {
        Rect rect = new Rect(50, 50, 500, 500);
        
       
        if (_currentMeshFilter)
        {
            GUI.Label(rect, $"Vertex Count: {_verticiesCount}");
            rect.y += 25;
            GUI.Label(rect, $"Triangles Count: {_trianglesCount}");
            rect.y += 25;
            GUI.Label(rect, $"UV Count: {_uvCount}");
        }

    }
#endif

}
