using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleRemoveTool : PlanTool<RectanglesPlan>
{
    public RectangleRemoveTool(RectanglesPlan plan) : base(plan) { }


    public override void ToolInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RemoveRectangle();
        }
    }

    void RemoveRectangle()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
        {
            RectangleMesh rm = hit.collider.GetComponent<RectangleMesh>();
            if (rm)
            {
                _plan.RemoveRectangle(rm.Rectangle);
            }
        }
    }

}
