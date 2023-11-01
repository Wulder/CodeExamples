using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine.Events;

[Serializable]
public class ReconstructionSaver
{
    private Point[][] _buffers;
    private PointVisualizer[] _visualizers;
    private double _saveProgress;

    //for load
    private PointVisualizerInfo[] _visualizersInfo;

    private ReconstructionInfo _reconstructionInfo;
    public Point[][] Buffers => Buffers;
    public PointVisualizer[] Visualizers => _visualizers;

    public PointVisualizerInfo[] VisualizersInfo => _visualizersInfo;
    public ReconstructionInfo ReconstructionInfo => _reconstructionInfo;
    public double SaveProgress => _saveProgress;



    public ReconstructionSaver(ReconstructionInfo recInfo, PointVisualizer[] visualizers)
    {
        _visualizers = visualizers;
        _buffers = new Point[recInfo.PointBuffer.SubBuffers.Count][];
        _reconstructionInfo = recInfo;
        for (int i = 0; i < recInfo.PointBuffer.SubBuffers.Count; i++)
        {

            Point[] points = new Point[recInfo.PointBuffer.SubBufferSize];
            recInfo.PointBuffer.SubBuffers[i].GetData(points, 0, 0, recInfo.PointBuffer.SubBufferSize);

            _buffers[i] = points;


        }

    }

    public ReconstructionSaver() { }



    async public void SaveReconstructionData(string path, UnityEvent callback)
    {
        _saveProgress = 0;
        await Task.Run(() =>
        {

            FileStream fs = new FileStream(path, FileMode.Create);

            BinaryWriter bw = new BinaryWriter(fs);

            //запись информации о структуре октодерева
            WriteReconstructionInfo(bw, _reconstructionInfo);

            //запись октодерева---------
            WriteArray(_reconstructionInfo._hashMap, _reconstructionInfo._hashMap.Length, bw);
            for (int i = 0; i < _reconstructionInfo._lvls.Length; i++)
            {
                bw.Write(_reconstructionInfo._pointers[i + 1]);
                WriteArray(_reconstructionInfo._lvls[i], _reconstructionInfo._pointers[i + 1], bw);

            }
            //-----------------------------------

            //запись информации о буферах
            bw.Write(_buffers.Length);
            bw.Write(_reconstructionInfo.PointBuffer.SubBufferSize);
            //запись самих буферов

            for (int i = 0; i < _buffers.Length; i++)
            {
                int length = Mathf.Clamp(_reconstructionInfo.LastPointBufferPointer - _reconstructionInfo.PointBuffer.SubBufferSize * i, 0, _reconstructionInfo.PointBuffer.SubBufferSize);
                bw.Write(length);

                WriteSubBuffer(bw, _buffers[i], length);


            }

            //запись информации о визуалайзерах (vfx графов)
            bw.Write(_visualizers.Length);
            //запись информации о каждом визуалайзере
            for (int i = 0; i < _visualizers.Length; i++)
            {
                WriteVisualizer(bw, _visualizers[i].VisualizerInfo);

            }


            fs.Close();
        });
        Debug.Log($"Reconstruction saved in {path}");
        callback?.Invoke();

    }

    public void ReadReconstructionData(string path)
    {

        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryReader br = new BinaryReader(fs);

        _reconstructionInfo = ReadReconstructionInfo(br);
        _reconstructionInfo._hashMap = new int[_reconstructionInfo.RootSize * _reconstructionInfo.RootSize * _reconstructionInfo.RootSize];
        ReadArray(ref _reconstructionInfo._hashMap, _reconstructionInfo._hashMap.Length, br);
        _reconstructionInfo._lvls = new int[_reconstructionInfo.OctreeDepth][];
        for (int i = 0; i < _reconstructionInfo.OctreeDepth; i++)
        {
            int length = br.ReadInt32();
            _reconstructionInfo._lvls[i] = new int[length];
            ReadArray(ref _reconstructionInfo._lvls[i], length, br);
        }

        int buffersCount = br.ReadInt32();
        int buffersSize = br.ReadInt32();

        PointBuffer pointBuffer = new PointBuffer(buffersSize);
        _reconstructionInfo.SetPointBuffer(pointBuffer);
        pointBuffer.CreateSubBuffer();


        for (int i = 0; i < buffersCount; i++)
        {
            Point[] subBuffer = new Point[buffersSize];
            int size = br.ReadInt32();
            subBuffer = ReadSubBuffer(br, size);
            _reconstructionInfo.PointBuffer.SubBuffers[i].SetData(subBuffer, 0, 0, size);

            if (i < buffersCount - 1)
                pointBuffer.CreateSubBuffer();

        }


        int visualizersCount = br.ReadInt32();
        _visualizersInfo = new PointVisualizerInfo[visualizersCount];

        for (int i = 0; i < _visualizersInfo.Length; i++)
        {
            _visualizersInfo[i] = ReadVisualizerInfo(br);
        }

        fs.Close();




    }

    void WriteReconstructionInfo(BinaryWriter bw, ReconstructionInfo info)
    {
        bw.Write(info.OccupiedPoints);
        bw.Write(info.LastPointBufferPointer);
        bw.Write(info.ChunkDensity);
        bw.Write(info.Density);
        bw.Write(info.PivotOffset.x);
        bw.Write(info.PivotOffset.y);
        bw.Write(info.PivotOffset.z);
        bw.Write(info.Pivot.x);
        bw.Write(info.Pivot.y);
        bw.Write(info.Pivot.z);
        bw.Write(info.OctreeDepth);
        bw.Write(info.RootSize);

    }

    ReconstructionInfo ReadReconstructionInfo(BinaryReader br)
    {
        ReconstructionInfoData recInfo = new ReconstructionInfoData();

        recInfo.OccupiedPoints = br.ReadInt32();
        recInfo.LastPointBufferPointer = br.ReadInt32();
        recInfo.ChunkDensity = br.ReadInt32();
        recInfo.Density = br.ReadInt32();
        recInfo.PivotOffset.x = br.ReadSingle();
        recInfo.PivotOffset.y = br.ReadSingle();
        recInfo.PivotOffset.z = br.ReadSingle();
        recInfo.Pivot.x = br.ReadSingle();
        recInfo.Pivot.y = br.ReadSingle();
        recInfo.Pivot.z = br.ReadSingle();
        recInfo.OctreeDepth = br.ReadInt32();
        recInfo.RootSize = br.ReadInt32();

        ReconstructionInfo result = new ReconstructionInfo(recInfo);
        return result;
    }

    void WriteArray(int[] arr, int length, BinaryWriter bw)
    {
        for (int i = 0; i < length; i++)
            bw.Write(arr[i]);
    }

    void ReadArray(ref int[] arr, int length, BinaryReader br)
    {
        for (int i = 0; i < length; i++)
        {
            arr[i] = br.ReadInt32();
        }
    }

    void WriteSubBuffer(BinaryWriter bw, Point[] subbuffer, int length)
    {
        for (int i = 0; i < length; i++)
        {
            bw.Write(subbuffer[i].Position.x);
            bw.Write(subbuffer[i].Position.y);
            bw.Write(subbuffer[i].Position.z);

            bw.Write(subbuffer[i].Color.r);
            bw.Write(subbuffer[i].Color.g);
            bw.Write(subbuffer[i].Color.b);
            bw.Write(subbuffer[i].Color.a);

            bw.Write(subbuffer[i].X);
            bw.Write(subbuffer[i].Y);
            bw.Write(subbuffer[i].RawDepthValue);
            bw.Write(subbuffer[i].ProcessedDepthValue);
            _saveProgress += 1.0 / length;
        }
    }

    Point[] ReadSubBuffer(BinaryReader br, int size)
    {
        Point[] result = new Point[size];

        for (int i = 0; i < size; i++)
        {
            result[i].Position.x = br.ReadSingle();
            result[i].Position.y = br.ReadSingle();
            result[i].Position.z = br.ReadSingle();

            result[i].Color.r = br.ReadSingle();
            result[i].Color.g = br.ReadSingle();
            result[i].Color.b = br.ReadSingle();
            result[i].Color.a = br.ReadSingle();

            result[i].X = br.ReadInt32();
            result[i].Y = br.ReadInt32();
            result[i].RawDepthValue = br.ReadInt32();
            result[i].ProcessedDepthValue = br.ReadInt32();
        }

        return result;

    }

    void WriteVisualizer(BinaryWriter bw, PointVisualizerInfo info)
    {
        bw.Write(info.BoundCenter.x);
        bw.Write(info.BoundCenter.y);
        bw.Write(info.BoundCenter.z);

        bw.Write(info.BoundSize.x);
        bw.Write(info.BoundSize.y);
        bw.Write(info.BoundSize.z);

        bw.Write(info.PointsCount);
        bw.Write(info.Offset);
        bw.Write(info.VisualizerNumber);
        bw.Write(info.GBufferIndex);
    }

    PointVisualizerInfo ReadVisualizerInfo(BinaryReader br)
    {
        PointVisualizerInfo result = new PointVisualizerInfo();

        result.BoundCenter.x = br.ReadSingle();
        result.BoundCenter.y = br.ReadSingle();
        result.BoundCenter.z = br.ReadSingle();

        result.BoundSize.x = br.ReadSingle();
        result.BoundSize.y = br.ReadSingle();
        result.BoundSize.z = br.ReadSingle();

        result.PointsCount = br.ReadInt32();
        result.Offset = br.ReadInt32();
        result.VisualizerNumber = br.ReadInt32();
        result.GBufferIndex = br.ReadInt32();

        return result;
    }
}


