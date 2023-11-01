using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class RectanglesObjectsCreatorTool : PlanTool<RectanglesPlan>
{

    private PlanObject _object;

    private Vector2 _objectSize, _objectPosition;

    public Vector2 ObjectsSize => _objectSize;
    public Vector2 ObjectsPosition => _objectPosition;

    private GameObject _objectPrefab;

    private HoleCutter _cutter;

    private ObjectsCreatorUI _ui;

    private RectangleMesh _selectedMesh;
    private RaycastHit _hit;
    private GameObject _objectPreview;
    public RectanglesObjectsCreatorTool(RectanglesPlan plan) : base(plan)
    {
        _cutter = new HoleCutter();
    }

    public override void ToolInput()
    {
        base.ToolInput();

        Select();

        if (Input.GetMouseButtonDown(0))
        {
            AddObject();
        }
    }

    public override void Enable()
    {
        base.Enable();
        _objectSize = Vector2.one;
        _objectPosition = Vector2.zero;

        var gm = Resources.Load("ToolsUI/Plans/Utilities/ObjectsCreatorUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(gm).GetComponent<ObjectsCreatorUI>();
        _ui.Init(this);
    }

    public void SetObjectSize(Vector2 s)
    {
        _objectSize = s;
        _object.SetSize(s);
        ResetGizmoObject();
        
    }

    public void SetObjectPos(Vector2 p)
    {
        _objectPosition = p;
        _object.SetPosOffset(p);
        ResetGizmoObject();
    }

    void ResetGizmoObject()
    {
        if(_objectPreview) GameObject.Destroy(_objectPreview);
    }
    public void SetObject(GameObject obj)
    {
        var planObj = obj.GetComponent<PlanObject>();
        if (planObj)
        {
            _object = planObj;
            _objectPrefab = obj;
        }
    }

    void AddObject()
    {
        if (_selectedMesh != null)
        {
            var hole = new RectangleHole();
            hole.Normal = _hit.normal.y != 0 ? Vector3.forward : _hit.normal;
            hole.Size = _object.GetHoleSize();
           
            

            hole.Position = _object.GetPosOnWall(_selectedMesh, _hit) + _objectPosition;

            _selectedMesh.GetComponent<MeshFilter>().mesh = _cutter.CutHole(_selectedMesh, hole);

            var gm = GameObject.Instantiate(_object);
            gm.transform.position = _selectedMesh.transform.position + _selectedMesh.Mesh.bounds.center + PlanObject.GetOffsetInWalll(hole.Position,_hit);
            gm.transform.localScale = _object.GetObjectTransfromScale();
            gm.transform.rotation = Quaternion.LookRotation(_hit.normal);
        }
    }

    void Select()
    {
        if(!_objectPrefab) return;

        _selectedMesh = PlanTool<RectanglesPlan>.SelectMesh(out _hit);

        if (_selectedMesh != null && _hit.normal.y == 0)
        {
            if(!_objectPreview)
            {
                _objectPreview = GameObject.Instantiate(_objectPrefab);
                _objectPreview.transform.localScale = _object.GetObjectTransfromScale();
            }
            _objectPreview.transform.position = _object.GetVec3OnWall(_selectedMesh, _hit) + PlanObject.GetOffsetInWalll(_objectPosition,_hit);
           
            _objectPreview.transform.rotation = Quaternion.LookRotation(_hit.normal);
        }
        else
        {
            if(_objectPreview) GameObject.Destroy(_objectPreview);

            _selectedMesh = null;
        }
    }
}
