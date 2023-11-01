using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Exoa.Designer;

public class ToolManager
{


    private static ToolManager instance;
    public static ToolManager Instance => instance;

    private Tool _currentTool;

    public Tool CurrentTool => _currentTool;

    private ApplicationController _appController;

    public ToolManager(ApplicationController appController)
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("Instance of ToolManager is already exist!");
            return;
        }

        _appController = appController;
        appController.SubscribeOnUpdate(Run);
        appController.SubscribeOnGizmos(DrawGizmos);
        appController.SubscribeOnGUI(DrawGUI);
    }
    public void SetTool(Tool tool)
    {
        _currentTool?.Disable();

        _currentTool = tool;
        _currentTool.Enable();
        Debug.Log($"Set new tool \"{tool.GetType().Name}\"");
    }
    public void Run()
    {
        _currentTool?.ToolInput();
    }

    private void DrawGizmos()
    {
        
        _currentTool?.DrawGizmos();
    }

    private void DrawGUI()
    {
        _currentTool?.DrawGUI(); 
    }

    public void SetFloor()
    {
        Debug.Log("Selected tool \"SetFloor\"");
        SetTool(new FloorAlignTool(_appController.MainMesh));
    }

    public void SetAngle()
    {
        Debug.Log("Selected tool \"SetAngle\"");
        SetTool(new AngleAlignTool());
    }

    public void SetCutWalls()
    {
        Debug.Log("Selected tool \"CutWalls\"");
        SetTool(new MeshSlicerTool());
    }

    public void SetSubtractorTool()
    {
        Debug.Log("Selected tool \"Subtractor\"");
        SetTool(new MeshSubtractorTool());
    }

    public void SetModelPlanTool()
    {
        Debug.Log("Selected tool \"ModelPlan\"");
        SetTool(new PlanEditorTool());
    }
}
