using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MeshShaderSlicer : OldTool
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private float _startSlicingOffset = 0.25f;
   
    public override void Enable()
    {
        base.Enable();
        
        _ui.SetActive(true);
    }

    public override void Disable()
    {
        base.Disable();
      
        _ui.SetActive(false);
    }

    

    public void SetDownThreshold(float val)
    {
        float sliceValue = 0;
        float min, max;
        var center = MeshSelector.Collider.bounds.center;
        Bounds bounds = MeshSelector.Collider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y - _startSlicingOffset;
        max = bounds.max.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y;
        sliceValue = min + (max + Mathf.Abs(min)) * val;
        MeshSelector.Renderer.materials[0].SetFloat("_DownThreshold", sliceValue);
    }

    public void SetUpThreshold(float val)
    {
        float sliceValue = 0;
        float min, max;
        Bounds bounds = MeshSelector.Collider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y;
        max = bounds.max.y - bounds.center.y + (bounds.center - MeshSelector.SelectedMesh.transform.position).y + _startSlicingOffset;
        sliceValue = min + (max + Mathf.Abs(min))*val;
        MeshSelector.Renderer.materials[0].SetFloat("_UpThreshold", sliceValue);
    }
}
