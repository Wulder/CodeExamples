using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDebugger : Tool
{

    [SerializeField] private PointSelector _pSelector;
    private void OnEnable()
    {
        _pSelector.OnSelectedNewPoint += SelectNewPoint;
    }

    private void OnDisable()
    {
        _pSelector.OnSelectedNewPoint -= SelectNewPoint;
    }

    void SelectNewPoint(RaycastHit hit)
    {
        int index = int.Parse(hit.collider.gameObject.name);

        Point[] point = new Point[1];
        _recInfo.PointBuffer.GetData(point, 0, index, 1);
       
        
    }
}
