using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrajectoryRender : MonoBehaviour
{
    [SerializeField] private VoxelReconstruction _reconstruction;
    [SerializeField] private LineRenderer _lineRenderer;
    

    private int _index, _frame;

    private void OnEnable()
    {
        _reconstruction.UpdateData += UpdateLine;
    }

    private void OnDisable()
    {
        _reconstruction.UpdateData -= UpdateLine;
    }


    void UpdateLine(DataFrame frame)
    {
        if(_frame%50 == 0)
        {
            _lineRenderer.SetPosition(_index, frame.Position);
            _index++;
        }

        _frame++;
    }
}
