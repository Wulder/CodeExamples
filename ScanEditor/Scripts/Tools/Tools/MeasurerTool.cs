using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeasurerTool : Tool
{
    MeasureUI _ui;

    private Measuring _currentMeasuring;

    public override void Enable()
    {
        GameObject gm = Resources.Load("ToolsUI/MeasureUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(gm).GetComponent<MeasureUI>();
        _ui.Init(this);
    }

    public override void Disable()
    {
        GameObject.Destroy(_ui.gameObject);
    }
    public override void ToolInput()
    {
        bool blockedByIMGUI = GUIUtility.hotControl != 0;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !blockedByIMGUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
            {
                if (_currentMeasuring.p0 == Vector3.zero)
                {
                    _currentMeasuring.p0 = hit.point;
                }
                else
                {
                    _currentMeasuring.p1 = hit.point;
                    AddLine();
                }
            }
        }
    }

    void AddLine()
    {
        MeasuringLines.AddLine(_currentMeasuring);
        _currentMeasuring = new Measuring();
    }
}
