using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlanDoor : PlanObject
{

    public override Vector2 GetHoleSize()
    {
        return new Vector2(transform.localScale.x * _size.x, transform.localScale.y * _size.y);
    }
    public override Vector3 GetVec3OnWall(RectangleMesh mesh, RaycastHit hit)
    {
        Vector3 result = hit.point;

        float yPos = mesh.Mesh.bounds.min.y + (transform.localScale.y  * _size.y)/2;
        result.y = yPos;

        return result;
    }

    public override Vector2 GetPosOnWall(RectangleMesh mesh, RaycastHit hit)
    {
        Vector2 result = Vector2.zero;
        var normal = hit.normal;

        Vector3 worldWallCenter = mesh.transform.position + mesh.Mesh.bounds.center;
      //  float yPos = mesh.Mesh.bounds.min.y + (transform.localScale.y * _size.y) / 2;
        float yPos = -mesh.Mesh.bounds.size.y/2 + (transform.localScale.y * _size.y) / 2;
        

        if (normal == Vector3.right || normal == Vector3.left)
        {
            result = new Vector2(hit.point.z - worldWallCenter.z, yPos);

        }
        else if (normal == Vector3.forward || normal == Vector3.back)
        {
            result = new Vector2(hit.point.x - worldWallCenter.x, yPos);
        }

        return result;
    }
}


