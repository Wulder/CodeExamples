using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanEditorTool : Tool
{
    private static Plan _plan;
    private WallEditorPlane _editorPlane;
    private Camera _camera;

    private Vector3 _positionOnPlane;

    private ToolUI<PlanEditorTool> _ui;

    public Plan Plan => _plan;

    public override void Enable()
    {
        var gm = Resources.Load("Tools/WallsEditor/WallEditorPlane", typeof(GameObject)) as GameObject;
        _editorPlane = GameObject.Instantiate(gm).GetComponent<WallEditorPlane>();

        

        _camera = Camera.main;

        if (_plan) return;

        gm = Resources.Load("ToolsUI/PlanEditorUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(gm).GetComponent<PlanEditorUI>();
        _ui.Init(this);

        if(_plan)
        _plan.ShowUI();
    }
    public override void Disable()
    {
        if (_editorPlane)
            GameObject.Destroy(_editorPlane.gameObject);
        

        if (_ui)
            GameObject.Destroy(_ui.gameObject);

        _plan.HideUI();
    }
    public override void ToolInput()
    {
        bool blockedByIMGUI = GUIUtility.hotControl != 0;
        if (!EventSystem.current.IsPointerOverGameObject() && !blockedByIMGUI)
        {
            UpdatePositionOnPlane();
            

            if (!_plan) return;
            _plan.UpdatePositionOnPlane(_positionOnPlane);
            _plan.PlanInput();
        }
    }
    void UpdatePositionOnPlane()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
        RaycastHit hit = new RaycastHit();

        Vector3 pos;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, LayerMask.GetMask("WallsEditorPlane")))
        {
            Vector3 flooredPos = Vector3Int.FloorToInt(hit.point);

            pos = hit.point - flooredPos;

            float x = pos.x;
            float z = pos.z;

            x = Mathf.FloorToInt(x / _editorPlane.Snap) * _editorPlane.Snap;
            z = Mathf.FloorToInt(z / _editorPlane.Snap) * _editorPlane.Snap;

            pos.x = x;
            pos.z = z;
            pos += flooredPos;
            pos.y = 0;

            _positionOnPlane = pos;
        }
    }

    public void CreatePlan(PlanType type)
    {
        if (_plan) return;
        switch (type)
        {
            case PlanType.Walls:
                {
                    var gm = Resources.Load("Tools/WallsEditor/WallsPlan", typeof(GameObject)) as GameObject;
                    _plan = GameObject.Instantiate(gm).GetComponentInParent<WallsPlan>();
                    break;
                }
            case PlanType.Rectangles:
                {
                    var gm = Resources.Load("Tools/RectangleEditor/RectanglesPlan", typeof(GameObject)) as GameObject;
                    _plan = GameObject.Instantiate(gm).GetComponentInParent<RectanglesPlan>();
                    break;
                }
        }

    }


}


public enum PlanType
{
    Walls,
    Rectangles
}
