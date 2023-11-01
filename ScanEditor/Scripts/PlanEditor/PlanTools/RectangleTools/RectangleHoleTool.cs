using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TriLibCore.Dae.Schema;
using Unity.VisualScripting;

public class RectangleHoleTool : PlanTool<RectanglesPlan>
{

    public event Action OnSelectMesh, OnCut;

    private HoleCutter _cutter;

    private HoleCutterUI _ui;

    private RectangleMesh _selectedMesh;
    private RectangleHole _selectedHole;
    private GameObject _subtractorGizmo;

    public RectangleHole SelectedHole => _selectedHole;
    public RectangleHoleTool(RectanglesPlan plan) : base(plan)
    {
        _cutter = new HoleCutter();

        var gm = Resources.Load("ToolsUI/Plans/Utilities/HoleCutterUI", typeof(GameObject)) as GameObject;

        _ui = GameObject.Instantiate(gm).GetComponent<HoleCutterUI>();
        _ui.Init(this);

    }

    public override void Enable()
    {
        base.Enable();
        OnCut += DestroySubtractor;
    }
    public override void Disable()
    {
        base.Disable();
        OnCut -= DestroySubtractor;
        if (_ui)
        {
            GameObject.Destroy(_ui.gameObject);
        }
    }

    public override void ToolInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectMesh();
        }
    }

    void SelectMesh()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
        {

            RectangleMesh rm = hit.collider.GetComponent<RectangleMesh>();
            if(rm)
            {
                _selectedMesh = rm;
                _selectedHole = new RectangleHole();

                _selectedHole.Normal = hit.normal.y != 0 ? Vector3.forward : hit.normal;
                _selectedHole.Size = Vector2.one;
                _selectedHole.Position = Vector2.zero;

                CreateSubtractorGizmo();

                OnSelectMesh?.Invoke();
            }
            
        }

        
    }

    public void CutHole()
    {
        
        if( _selectedHole != null & _selectedMesh != null)
        {
            var mesh = _cutter.CutHole(_selectedMesh, _selectedHole);
            _selectedMesh.GetComponent<MeshFilter>().mesh = mesh;
            _selectedMesh.AddHole(_selectedHole);
            OnCut?.Invoke();
        }

    }

    public void CreateSubtractorGizmo()
    {
        if(_subtractorGizmo) GameObject.Destroy(_subtractorGizmo);
        _subtractorGizmo =_cutter.CreateSubtracter(_selectedMesh, _selectedHole);
        
    }

    public void DestroySubtractor()
    {
        if(_subtractorGizmo) GameObject.Destroy( _subtractorGizmo);
    }

    public void UpdateSubtractor()
    {

        var meshBounds = _selectedMesh.GetComponent<MeshFilter>().mesh.bounds;
        if (_selectedHole.Normal == Vector3.right || _selectedHole.Normal == Vector3.left)
        {
            _subtractorGizmo.transform.position = _selectedMesh.transform.position + meshBounds.center + new Vector3(0, _selectedHole.Position.y, _selectedHole.Position.x);
            _subtractorGizmo.transform.localScale = new Vector3(meshBounds.size.x, _selectedHole.Size.y, _selectedHole.Size.x);
        }
        else if (_selectedHole.Normal == Vector3.forward || _selectedHole.Normal == Vector3.back)
        {
            _subtractorGizmo.transform.position = _selectedMesh.transform.position + meshBounds.center + new Vector3(_selectedHole.Position.x, _selectedHole.Position.y, 0);
            _subtractorGizmo.transform.localScale = new Vector3(_selectedHole.Size.x, _selectedHole.Size.y, meshBounds.size.z);
        }
    }

    public void UpdateHoleSize(Vector2 size)
    {
        _selectedHole.Size = size;
        UpdateSubtractor();
    }

    public void UpdateHolePos(Vector2 pos)
    {
        _selectedHole.Position = pos;
        UpdateSubtractor();
    }

}
