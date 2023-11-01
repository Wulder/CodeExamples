using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldToolsManager : MonoBehaviour
{
    private static OldToolsManager _instance;
    public static OldToolsManager Instance => _instance;

    private OldTool _currentTool;
    public OldTool CurrentTool => _currentTool;

    [SerializeField] private List<OldTool> _tools = new List<OldTool>();

    private void Awake()
    {
        if(_instance == null)
        _instance = this;
    }
    public void SelectNewTool(OldTool t)
    {
        _currentTool = t;
        t.Enable();
        foreach (OldTool tool in _tools)
        {
            if(tool != t)
            {
                tool.Disable();
            }
        }
    }
}
