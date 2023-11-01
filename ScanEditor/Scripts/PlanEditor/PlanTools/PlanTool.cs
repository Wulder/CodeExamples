using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanTool<PlanType>
{
    protected PlanType _plan;

    public PlanTool(PlanType plan)
    {
        _plan = plan;
    }

    public virtual void Enable()
    {
        Debug.Log(this.GetType().Name);
    }
    public virtual void Disable() { }

    public virtual void ToolInput()
    {

    }

    public static RectangleMesh SelectMesh(out RaycastHit outHit)
    {

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
        RaycastHit hit = new RaycastHit();
       
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, LayerMask.GetMask("Wall")))
        {
            outHit = hit;
            RectangleMesh rm = hit.collider.GetComponent<RectangleMesh>();
            if (rm)
            {
                return rm;
            }

        }
        outHit = hit;
        return null;
    }

    public static RectangleMesh SelectMesh()
    {
        RaycastHit hit;
        return SelectMesh(out hit);
    }
}
