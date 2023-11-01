using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ReconstructionInfo
{
    public static event Action<ReconstructionInfo> OnCreated;

    private PointBuffer _pointBuffer;

    private int _occupiedPoints;
    private int _lastPointBufferPointer;

    private int _rootSize;
    private int _chunksCount;
    private int _octreeDepth;
    private int _chunkDensity;
    private int _density;
    private Vector3 _pivot, _pivotOffset;

    public int[] _hashMap;
    public int[][] _lvls;
    public int[] _pointers;



    public PointBuffer PointBuffer => _pointBuffer;
    /// <summary>
    /// Общее количество точек в буфере
    /// </summary>
    public int TotalPoints => _lastPointBufferPointer;
    /// <summary>
    /// Количество точек имеющих значения (не нулевые)
    /// </summary>
    public int OccupiedPoints => _occupiedPoints;
    /// <summary>
    /// Количество чанков в облаке
    /// </summary>
    public int ChunksCount => _lastPointBufferPointer / (_chunkDensity * _chunkDensity * _chunkDensity);
    public int ChunkDensity => _chunkDensity;
    public int OctreeDepth => _octreeDepth;
    public int Density => _density;
    public int LastPointBufferPointer => _lastPointBufferPointer;

    public int RootSize => _rootSize;
    public Vector3 PivotOffset => _pivotOffset;
    public Vector3 Pivot => _pivot;

    public ReconstructionInfo(int rootSize, int octreeDepth, int blockDensity, PointBuffer pointBuffer)
    {
        _pointBuffer = pointBuffer;
        _octreeDepth = octreeDepth;
        _chunkDensity = blockDensity;
        _density = _chunkDensity * (int)Mathf.Pow(2, _octreeDepth);
        _pivotOffset = Vector3.zero;
        _pivot = Vector3.zero;
        _rootSize = rootSize;
        _hashMap = new int[_rootSize * _rootSize * _rootSize];
        _lvls = new int[_octreeDepth][];
        
        Array.Fill(_hashMap, -1);
        for (int i = 0; i < _octreeDepth; i++)
        {
            _lvls[i] = new int[_hashMap.Length * 10];
            Array.Fill(_lvls[i], -1);
        }
        _pointers = new int[_octreeDepth + 1];

        Array.Fill(_pointers, 0);

        OnCreated?.Invoke(this);
    }

    public ReconstructionInfo(ReconstructionInfoData info)
    {
        _occupiedPoints = info.OccupiedPoints;
        _lastPointBufferPointer = info.LastPointBufferPointer;
        _octreeDepth = info.OctreeDepth;
        _chunkDensity = info.ChunkDensity;
        _density = info.Density;
        _pivotOffset = info.PivotOffset;
        _pivot = info.Pivot;
        _rootSize = info.RootSize;

        OnCreated?.Invoke(this);
    }


    public void SetLastBufferPointer(int pointer)
    {
        _lastPointBufferPointer = pointer;
    }

    public void AddOccupiedPoints(int count)
    {
        _occupiedPoints += count;
    }

    public void SetPointBuffer(PointBuffer buffer)
    {
        _pointBuffer = buffer;
    }

    public void SetPivotOffset(Vector3 pOffset)
    {
        _pivotOffset = pOffset;
    }

    public void SetPivot(Vector3 p)
    {
        _pivot = p;
    }
}

public struct ReconstructionInfoData
{
    public int OccupiedPoints;
    public int LastPointBufferPointer;

    public int OctreeDepth;
    public int ChunkDensity;
    public int Density;
    public Vector3 PivotOffset;
    public Vector3 Pivot;
    public int RootSize;
}
