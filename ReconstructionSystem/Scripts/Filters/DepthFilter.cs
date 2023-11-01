using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DepthFilter : MonoBehaviour
{

    [SerializeField] private protected ComputeShader _shader;
    ComputeBuffer buffer;

    private void OnEnable()
    {
        buffer = new ComputeBuffer(640 * 480, Marshal.SizeOf(typeof(int)));
    }

    private void OnDisable()
    {
        buffer.Release();
    }

    public void Execute(ref uint[] depth, int width = 640, int height = 480)
    {
        InitShader();

        if(width != 640 || height != 480)
        {
            buffer = new ComputeBuffer(width * height, Marshal.SizeOf(typeof(int)));
        }

        int kernel = _shader.FindKernel("Filter");
        
        buffer.SetData(depth);
        _shader.SetBuffer(kernel,"Depth", buffer);
        _shader.Dispatch(kernel, depth.Length / 1024, 1, 1);
        buffer.GetData(depth);
        buffer.Release();
        
    }

    protected virtual void InitShader() { }

}
