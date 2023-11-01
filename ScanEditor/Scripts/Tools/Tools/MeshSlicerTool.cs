using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlicerTool : Tool
{
    private MeshSlicerUI _ui;

    private float _startSlicingOffset = 0.25f;
    public override void Enable()
    {
        base.Enable();
        GameObject ui = Resources.Load("ToolsUI/MeshSlicerUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(ui).GetComponent<MeshSlicerUI>();
        
        _ui.Init(this);

        
    }

    public override void Disable()
    {
        base.Disable();
        GameObject.Destroy(_ui.gameObject);
    }

    public void SetDownThreshold(float val)
    {
        float sliceValue = 0;
        float min, max;
        var collider = ApplicationController.Instance.MainMesh.GetComponent<Collider>();
        var center = collider.bounds.center;
        Bounds bounds = collider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - ApplicationController.Instance.MainMesh.transform.position).y - _startSlicingOffset;
        max = bounds.max.y - bounds.center.y + (bounds.center - ApplicationController.Instance.MainMesh.transform.position).y;
        sliceValue = min + (max + Mathf.Abs(min)) * val;

        foreach(var mr in ApplicationController.Instance.MainMesh.GetComponentsInChildren<MeshRenderer>())
        {
            foreach(var mat in mr.materials)
            {
                mat.SetFloat("_DownThreshold", sliceValue);
            }
        }
        
        //ApplicationController.Instance.MainMesh.GetComponent<MeshRenderer>().materials[0].SetFloat("_DownThreshold", sliceValue);
    }

    public void SetUpThreshold(float val)
    {
        float sliceValue = 0;
        float min, max;
        var collider = ApplicationController.Instance.MainMesh.GetComponent <Collider>();
        Bounds bounds = collider.bounds;

        min = bounds.min.y - bounds.center.y + (bounds.center - ApplicationController.Instance.MainMesh.transform.position).y;
        max = bounds.max.y - bounds.center.y + (bounds.center - ApplicationController.Instance.MainMesh.transform.position).y + _startSlicingOffset;
        sliceValue = min + (max + Mathf.Abs(min)) * val;


        foreach (var mr in ApplicationController.Instance.MainMesh.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (var mat in mr.materials)
            {
                mat.SetFloat("_UpThreshold", sliceValue);
            }
        }

       // ApplicationController.Instance.MainMesh.GetComponent<MeshRenderer>().materials[0].SetFloat("_UpThreshold", sliceValue);
    }
}
