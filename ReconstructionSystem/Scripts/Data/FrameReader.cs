using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;


public abstract class FrameReader : MonoBehaviour
{

    public event Action OnDataReady;

    private DataFrame _data;
    private bool _dataReady = false;
    private Thread _thread;
    protected float _fx, _fy, _cx, _cy;
    

    public float Fx => _fx;
    public float Fy => _fy;
    public float Cx => _cx;
    public float Cy => _cy;

    [SerializeField] private int _timeOut;
    [SerializeField] private bool _showReadingFps;
    [SerializeField] private Vector3 _positionOffset;

    private void OnEnable()
    {
        _thread = new Thread(Run);
    }
    private void OnDisable()
    {
        Debug.Log($"{_thread.ThreadState}. Aborting frame reader thread...");
        _thread.Abort();
        Debug.Log($"{_thread.ThreadState}.");
    }
    private void Start()
    {
        Init();

        _thread.Start();
    }


    DateTime fpsTimer = DateTime.Now;
    int fps;
    private void Run()
    {
        while (true)
        {

            if (DateTime.Now.Subtract(fpsTimer).Seconds >= 1)
            {
                if (_showReadingFps)
                    Debug.Log(fps);

                fpsTimer = DateTime.Now;
                fps = 0;
            }


            if (!_dataReady)
            {
                PrepareData();
                fps++;
            }

            Thread.Sleep(_timeOut);

        }

    }
    public bool GetFrame(out DataFrame outData)
    {
        if (_dataReady)
        {
            outData = _data;
            _dataReady = false;
            return true;
        }
        else
        {
            outData = new DataFrame();
            return false;
        }
    }
    protected void PrepareData()
    {
        _data = CreateData();
        _data.Position += _positionOffset;
        _dataReady = true;
        OnDataReady?.Invoke();
    }



    protected virtual void Init() { }
    protected abstract DataFrame CreateData();
}
public struct DataFrame
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Color[] Color;
    public  uint[] Depth;
    public int[] Confidence;
}

public struct PoseFrame
{
    public Vector3 Position;
    public Quaternion Rotation;
}
