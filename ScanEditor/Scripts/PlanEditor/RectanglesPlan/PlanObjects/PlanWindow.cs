using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlanWindow : PlanObject
{
    [SerializeField] private float _clampY;
    public override Vector3 GetVec3OnWall(RectangleMesh mesh, RaycastHit hit)
    {
        Vector3 result = Vector3.zero;
        var normal = hit.normal;

        var yOnWall = _clampY + mesh.Mesh.bounds.center.y;
        if (normal == Vector3.right || normal == Vector3.left)
        {
            result = new Vector3(hit.point.x, yOnWall, hit.point.z);

        }
        else if (normal == Vector3.forward || normal == Vector3.back)
        {
            result = new Vector3(hit.point.x, yOnWall, hit.point.z);
        }
        return result;
    }

    public override Vector2 GetHoleSize()
    {
        return new Vector2(_size.x, _size.y);
    }

    

    public override Vector2 GetPosOnWall(RectangleMesh mesh, RaycastHit hit)
    {
        Vector2 result = Vector2.zero;
        var normal = hit.normal;

        Vector3 worldWallCenter = mesh.transform.position + mesh.Mesh.bounds.center;

        if (normal == Vector3.right || normal == Vector3.left)
        {
            result = new Vector2(hit.point.z - worldWallCenter.z, _clampY);

        }
        else if (normal == Vector3.forward || normal == Vector3.back)
        {
            result = new Vector2(hit.point.x - worldWallCenter.x, _clampY);
        }

        return result;

    }
}
