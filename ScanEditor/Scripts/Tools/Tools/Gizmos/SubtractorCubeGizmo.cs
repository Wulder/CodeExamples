using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtractorCubeGizmo : MonoBehaviour
{
    [SerializeField] private GameObject _gizmoObject;
    [SerializeField] private Transform _scaleFactorTransform;
    public void SetSize(Vector3 size)
    {
        _gizmoObject.transform.localScale = size;
    }

    public Vector3 GetScaleFactor()
    {
        return _scaleFactorTransform.localScale;
    }
}
