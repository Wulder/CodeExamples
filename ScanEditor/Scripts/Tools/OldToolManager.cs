using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldToolManager : MonoBehaviour
{
    [SerializeField] private List<OldTool> _toolList = new List<OldTool>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectTool(OldTool tool)
    {
        foreach (OldTool oldTool in _toolList)
        {
            oldTool.Disable();
        }

        tool.Enable();
    }
}
