using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolExecutor : MonoBehaviour
{
    private Tool _tool;

    private void Awake()
    {
        _tool = new PlanEditorTool();
    }

    private void OnEnable()
    {
        _tool?.Enable();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _tool?.ToolInput();
    }

    private void OnDrawGizmos()
    {
        _tool?.DrawGizmos();
    }

    private void OnGUI()
    {
        _tool?.DrawGUI();
    }
}
