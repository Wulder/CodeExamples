using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterThreshHold : DepthFilter
{
    [SerializeField] private float _filterTreshold;
    [SerializeField] private int  _near,_far, _width, _height;

    protected override void InitShader()
    {
        base.InitShader();
        _shader.SetFloat("filterTreshold", _filterTreshold);
        _shader.SetInt("near", _near);
        _shader.SetInt("far", _far);
        _shader.SetInt("width", _width);
        _shader.SetInt("height", _height);
    }
}
