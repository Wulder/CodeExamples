using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PreviewImages : MonoBehaviour
{
    [SerializeField] private VoxelReconstruction _reconstructionSystem;
    [SerializeField] private ComputeShader _shader;

    [SerializeField] private RenderTexture _colorTexture, _depthTexture;

    [SerializeField] private float _depthScale;


    private ComputeBuffer _colorBuffer, _depthBuffer;

    private void OnEnable()
    {
        if (_reconstructionSystem != null)
            _reconstructionSystem.UpdateData += UpdatePreviews;

        _colorBuffer = new ComputeBuffer(640 * 480, 4 * 4);
        _depthBuffer = new ComputeBuffer(640 * 480, 4 * 4);
        _depthTexture.enableRandomWrite = true;
        _colorTexture.enableRandomWrite = true;


    }

    private void OnDisable()
    {
        _reconstructionSystem.UpdateData -= UpdatePreviews;

        _colorBuffer.Release();
        _depthBuffer.Release();
    }

    public void UpdatePreviews(DataFrame frame)
    {
        _colorBuffer.SetData(frame.Color);
        _depthBuffer.SetData(frame.Depth);

        int kernel = _shader.FindKernel("Draw");

        _shader.SetTexture(kernel, "ColorPreview", _colorTexture);
        _shader.SetBuffer(kernel, "ColorBuffer", _colorBuffer);


        _shader.SetTexture(kernel, "DepthPreview", _depthTexture);
        _shader.SetBuffer(kernel, "DepthBuffer", _depthBuffer);

        _shader.SetFloat("depthScale", _depthScale);

        _shader.Dispatch(kernel, 640 * 480 / 1024, 1, 1);
    }
}
