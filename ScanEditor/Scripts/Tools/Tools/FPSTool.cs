using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTool : Tool
{
    private GameObject _fpsPrefab;
    private Camera _camera;
    private GameObject _fpsController;

    private Vector3 _startPoint;

    private FPSSubTool _subTool;

    public Camera Camera => _camera;
    public FPSTool()
    {
        GameObject gm = Resources.Load("Tools/FpsMode/FpsController", typeof(GameObject)) as GameObject;
        _fpsPrefab = gm;
        _camera = gm.GetComponentInChildren<Camera>();
    }

    public override void Enable()
    {
        SetSubTool(new FPSMesuarer());
    }
    public override void Disable()
    {
        _subTool?.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ApplicationController.Instance.MainCamera.enabled = true;
        if (_fpsController)
            GameObject.Destroy(_fpsController);

        
    }
    public override void ToolInput()
    {
        if (!_fpsController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectPoint();
            }
        }
        else
        {
            _subTool.ToolInput();
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ApplicationController.Instance._toolManager.SetTool(new EmptyTool());
            }
        }
    }

    void SetSubTool(FPSSubTool t)
    {
        _subTool?.Disable();
        _subTool = t;
        _subTool.Enable(this);
    }

    void SelectPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            _startPoint = hit.point;
            SpawnController();
        }
    }

    

    void SpawnController()
    {
        ApplicationController.Instance.MainCamera.enabled = false;
        _fpsController = GameObject.Instantiate(_fpsPrefab,_startPoint,Quaternion.identity);
    }

}
