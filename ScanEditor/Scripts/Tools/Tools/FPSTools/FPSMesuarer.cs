using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMesuarer : FPSSubTool
{
    private Measuring _currentMeasuring;
    public override void ToolInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
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

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            MeasuringLines.RemoveLastLine();
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            MeasuringLines.Clear();
        }
      
    }

    void AddLine()
    {
        MeasuringLines.AddLine(_currentMeasuring);
        _currentMeasuring = new Measuring();
    }
}
