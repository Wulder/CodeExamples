using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PointVisualizer : MonoBehaviour
{

    private Vector3 _center;

    private Transform _camera;
    [SerializeField] private VisualEffect _vfx;
    private bool _decreaseByDistance = false;

    private PointVisualizerInfo _visualizerInfo;

    [SerializeField] private int _pointCount;
    [SerializeField] private float _minDist, _step, _pointScale;

    private int _prevStepCount = 0;

    public PointVisualizerInfo VisualizerInfo => _visualizerInfo;
    void Start()
    {
        _camera = Camera.main.transform;

    }

    private void FixedUpdate()
    {
            if(Input.GetKeyDown(KeyCode.R)) { Reinit(1); }
            float distance = Vector3.Distance(_visualizerInfo.BoundCenter, _camera.position);

            float localDistance = distance - _minDist;
            int steps = (int)(localDistance / _step);
            if (_prevStepCount != steps)
            {
                if (_decreaseByDistance)
                {
                    if (steps <= 0)
                    {
                        Reinit(1);
                    }
                    else
                    {
                        Reinit(steps);
                    }
                }
               
            }
            _prevStepCount = steps;
        
    }

    void Reinit(int devider)
    {
        _vfx.SetInt(Shader.PropertyToID("pointCount"), _pointCount / devider);
        _vfx.SetInt(Shader.PropertyToID("step"), devider);
        _vfx.SetFloat(Shader.PropertyToID("pointScale"), _pointScale);

        _vfx.Reinit();

    }
    public void SetCenter(Vector3 center)
    {
        _visualizerInfo.BoundCenter = center;
        _vfx.SetVector3(Shader.PropertyToID("boundCenter"), center);
    }

    public void SetSize(Vector3 size)
    {
        _visualizerInfo.BoundSize = size;
        _vfx.SetVector3(Shader.PropertyToID("boundSize"), size);
    }
    public void Init(int pointerOffset, int _visualizerNumber, GraphicsBuffer buffer, int gBufferIndex, Vector3 boundCenter, Vector3 boundSize)
    {

        _visualizerInfo.Offset = pointerOffset;
        _visualizerInfo.VisualizerNumber = _visualizerNumber;
        _visualizerInfo.BoundCenter = boundCenter;
        _visualizerInfo.BoundSize = boundSize;
        _visualizerInfo.GBufferIndex = gBufferIndex;

        _vfx.SetInt(Shader.PropertyToID("pointerOffset"), _visualizerInfo.Offset);
        _vfx.SetInt(Shader.PropertyToID("visualizerNum"), _visualizerInfo.VisualizerNumber);
        _vfx.SetGraphicsBuffer(Shader.PropertyToID("pointBuffer"), buffer);
        _vfx.SetVector3(Shader.PropertyToID("boundCenter"), _visualizerInfo.BoundCenter);
        _vfx.SetVector3(Shader.PropertyToID("boundSize"), _visualizerInfo.BoundSize);
    }

    public void DecreseByDistance(bool v)
    {
        _decreaseByDistance = v;
    }
}

public struct PointVisualizerInfo
{
    public Vector3 BoundCenter;
    public Vector3 BoundSize;

    public int PointsCount;
    public int Offset;
    public int VisualizerNumber;
    public int GBufferIndex;
}
