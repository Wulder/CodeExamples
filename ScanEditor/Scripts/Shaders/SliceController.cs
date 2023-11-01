using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceController : MonoBehaviour
{
    [SerializeField] private float _upThreshold;
    [SerializeField] private float _downThreshold;
    [SerializeField] private float _startSlicingOffset = 0.25f;
    [SerializeField] private MeshRenderer _meshRenderer;


    [SerializeField] private MeshCollider _collider;
    [SerializeField] private Transform _transform;

    private Collider GetCollider { get { return MeshSelector.Collider != null ? MeshSelector.Collider : _collider; } }
    private Transform GetTransform { get { return MeshSelector.SelectedMesh != null ? MeshSelector.SelectedMesh.transform : _transform; } }
    //private void OnValidate()
    //{
    //    foreach(Material m in _meshRenderer.materials)
    //    {
    //        SetUpThreshold(_upThreshold, m);
    //        SetDownThreshold(_downThreshold, m);
    //    }
    //}

    public void SetDownThreshold(float val, Material mat)
    {
        float sliceValue = 0;
        float min, max;
        var center = GetCollider.bounds.center;
        Bounds bounds = GetCollider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - GetTransform.position).y - _startSlicingOffset;
        max = bounds.max.y - bounds.center.y + (bounds.center - GetTransform.position).y;
        sliceValue = min + (max + Mathf.Abs(min)) * val;
        mat.SetFloat("_DownThreshold", sliceValue);
    }

    public void SetUpThreshold(float val, Material mat)
    {
        float sliceValue = 0;
        float min, max;
        Bounds bounds = GetCollider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - GetTransform.position).y;
        max = bounds.max.y - bounds.center.y + (bounds.center - GetTransform.position).y + _startSlicingOffset;
        sliceValue = min + (max + Mathf.Abs(min)) * val;
       mat.SetFloat("_UpThreshold", sliceValue);
    }

    
}
