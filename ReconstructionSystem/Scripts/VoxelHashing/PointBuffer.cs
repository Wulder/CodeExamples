using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading.Tasks;



public class PointBuffer
{
    private List<GraphicsBuffer> _subBuffers = new List<GraphicsBuffer>();
    private int _subBufferSize;
    private ComputeShader _writeShader;
    private ComputeBuffer _writingPointsBuffer;



    public List<GraphicsBuffer> SubBuffers => _subBuffers;
    public int SubBufferSize => _subBufferSize;
    public PointBuffer()
    {
        _subBufferSize = (int)(SystemInfo.maxGraphicsBufferSize / (long)Marshal.SizeOf(typeof(Point)));
        
    }

    public PointBuffer(int subBufferSize)
    {
        _subBufferSize = subBufferSize;
        
    }

    public PointBuffer(ComputeShader shader, int pointsBufferSize)
    {
        _writeShader = shader;
        _writingPointsBuffer = new ComputeBuffer(pointsBufferSize, Marshal.SizeOf(typeof(PointData)));

        _subBufferSize = (int)(SystemInfo.maxGraphicsBufferSize / (long)Marshal.SizeOf(typeof(Point)));


        CreateSubBuffer();
        CreateSubBuffer();
        CreateSubBuffer();
        int kernel = _writeShader.FindKernel("WritePoints");
        _writeShader.SetInt("gBufferSize", SubBufferSize);
        _writeShader.SetBuffer(kernel, "gBuffer1", SubBuffers[0]);
        _writeShader.SetBuffer(kernel, "gBuffer2", SubBuffers[1]);
        _writeShader.SetBuffer(kernel, "gBuffer3", SubBuffers[2]);


    }


    public GraphicsBuffer CreateSubBuffer()
    {

        GraphicsBuffer b = new GraphicsBuffer(GraphicsBuffer.Target.Raw, _subBufferSize, Marshal.SizeOf(typeof(Point)));
        SubBuffers.Add(b);
        Debug.Log($"Created new graphics buffer. Size: {_subBufferSize}");

        return b;
    }

    public void SetData(Array data, int managedBufferStart, int graphicsBufferStart, int count)
    {
        if (graphicsBufferStart + count > _subBufferSize * _subBuffers.Count)
        {
            CreateSubBuffer();
        }

        int subBufferNum = GetSubBuferByPointer(graphicsBufferStart);
        int slice = (graphicsBufferStart + count) - subBufferNum * _subBufferSize;
        int remainder = slice > _subBufferSize ? slice % _subBufferSize : 0;

        _subBuffers[subBufferNum].SetData(data, 0, slice - count, count - remainder);

        if (remainder > 0)
            _subBuffers[subBufferNum + 1].SetData(data, 0, 0, remainder);



    }

    public int SetData(Array data, int count)
    {

        ComputeBuffer ocuppiedPointsBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(int)));
        ocuppiedPointsBuffer.SetData(new int[1] { 0 });

        int kernel = _writeShader.FindKernel("WritePoints");
        _writingPointsBuffer.SetData(data, 0, 0, count);
        _writeShader.SetBuffer(kernel, "writingPoints", _writingPointsBuffer);
        _writeShader.SetBuffer(kernel, "ocuppiedPoints", ocuppiedPointsBuffer);
        
        _writeShader.Dispatch(kernel, count / 1024, 1, 1);

        int[] ocuppiedPoints = new int[1] { 0 };
        ocuppiedPointsBuffer.GetData(ocuppiedPoints,0,0,1);
        ocuppiedPointsBuffer.Release();

        return ocuppiedPoints[0];


    }

    public void GetData(Array data, int managedBufferStart, int graphicsBufferStart, int count)
    {
        if (graphicsBufferStart + count > _subBufferSize * _subBuffers.Count)
        {
            CreateSubBuffer();
        }

        int startSubBuffer = GetSubBuferByPointer(graphicsBufferStart);
        int endSubBufer = GetSubBuferByPointer(graphicsBufferStart + count);

        int startPointer = graphicsBufferStart - _subBufferSize * startSubBuffer;

        int dataPointer = 0;

        int dataReminder = count;

        for (int i = startSubBuffer, j = 0; i <= endSubBufer; i++, j++)
        {
            int dataPartCount = dataReminder;

            if (startPointer + dataPartCount > _subBufferSize)
            {
                dataPartCount = _subBufferSize - startPointer;
            }

            _subBuffers[i].GetData(data, dataPointer, startPointer, dataPartCount);

            dataPointer += dataPartCount;
            startPointer = 0;
            dataReminder -= dataPartCount;
        }

    }
    public void Release()
    {
        
     

        for(int i =0; i < _subBuffers.Count; i++)
        {
            _subBuffers[i].Release();
        }

        if(_writingPointsBuffer != null) 
            _writingPointsBuffer.Release();
    }

    public int GetSubBuferByPointer(int pointer)
    {
        return pointer / _subBufferSize;
    }


}
