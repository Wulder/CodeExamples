using System.Collections;
using System.Collections.Generic;
using TriLibCore.Dae.Schema;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlanObject : MonoBehaviour
{


    [SerializeField] protected Vector2 _size;
    [SerializeField] protected Vector2 _posOffset;
    public Vector2 Size => _size;

    public void SetSize(Vector2 size)
    {
        _size = size;
    }

    public void SetPosOffset(Vector2 offset)
    {
        _posOffset = offset;
    }

    public virtual Vector2 GetHoleSize()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetObjectTransfromScale()
    {
        return new Vector3(transform.localScale.x * _size.x, transform.localScale.y * _size.y, transform.localScale.z);
    }
    public virtual Vector2 GetPosOnWall(RectangleMesh mesh, RaycastHit hit)
    {
        throw null;
    }

    public virtual Vector3 GetVec3OnWall(RectangleMesh mesh, RaycastHit hit)
    {
        throw null;
    }

    public static Vector3 GetOffsetInWalll(Vector2 pos, RaycastHit hit)
    {
        Vector3 offset = Vector3.zero;

        if (hit.normal == Vector3.right || hit.normal == Vector3.left)
        {
            offset = new Vector3(0,pos.y,pos.x);
        }
        else if (hit.normal == Vector3.forward || hit.normal == Vector3.back)
        {
            offset = new Vector3(pos.x, pos.y, 0);
        }

        return offset;
    }
}
