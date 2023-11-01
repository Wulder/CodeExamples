using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FramesReader : MonoBehaviour
{
    private protected UnityEngine.Color[] _colorBuffer;
    private protected uint[] _depthBuffer;
    private protected int[] _confidenceBuffer;

    private protected float _fx, _fy, _cx, _cy;

    public UnityEngine.Color[] ColorBuffer => _colorBuffer;
    public uint[] DepthBuffer => _depthBuffer;
    public int[] ConfidenceBuffer => _confidenceBuffer;
    public float Fx => _fx;
    public float Fy => _fy;
    public float Cx => _cx;
    public float Cy => _cy;


    public abstract void GetNext();
    public abstract void Init(string path, int startPointer = 0);
    
}
