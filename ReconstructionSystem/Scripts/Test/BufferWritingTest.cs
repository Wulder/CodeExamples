using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class BufferWritingTest : MonoBehaviour
{
    [SerializeField] private int _dataSize, _iterations;

    private GraphicsBuffer _buffer;

    void Start()
    {
        _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Append, (2147483647) / Marshal.SizeOf(typeof(Point)), Marshal.SizeOf(typeof(Point)));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            for(int i = 0; i < _iterations; i++)
                WriteData();
        }

        if (Input.GetKey(KeyCode.R))
        {
            for (int i = 0; i < _iterations; i++)
                ReadData();
            
        }
    }

    void WriteData()
    {
        Point[] points = new Point[_dataSize];
        _buffer.SetData(points, 0, 0, _dataSize);
    }

    void ReadData()
    {
        Point[] points = new Point[_dataSize];
        _buffer.GetData(points, 0, 0, _dataSize);
    }
}
