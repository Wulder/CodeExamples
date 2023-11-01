using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEditorSwitcher : OldTool
{
    [SerializeField] private GameObject _floorEditorObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Enable()
    {
        _floorEditorObject.SetActive(true);
        base.Enable();
        
    }

    public override void Disable()
    {
        _floorEditorObject.SetActive(false);
        base.Disable();
        
    }
}
