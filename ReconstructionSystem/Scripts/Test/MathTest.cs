using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class MathTest : MonoBehaviour
{

    [SerializeField] private int _width, _height, _depth;
    [SerializeField] private int K;
    [SerializeField] Vector3Int _testValue;

    int[] results = new int[8 * 8 * 8];

    [Button]
    void ResetArray()
    {
        results = new int[_width * _height * _depth];
        Array.Fill(results, -1);
    }

    [Button]
    void OwnValue()
    {
        Debug.Log(Calculate(_testValue.x, _testValue.y, _testValue.z));
    }

    int Calculate(int x,int y, int z)
    {
        return (_height * y + x) + (_height*_depth) * z;
    }

    [Button]
    void Simulation()
    {
        
        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                for(int k = 0; k < _depth; k++)
                {
                    int key = Calculate(i, k, j);
                    Debug.Log($"key: {key} | {i}|{k}|{j}");
                    if (results[key] > -1)
                    {
                        Debug.LogWarning($"Collision at {key}; {i}|{k}|{j}");
                        return;
                    }
                    results[key] = 1;
                }
            }
        }
    }

    bool CheckArray(int key)
    {
        for(int i = 0; i < results.Length; i++)
        {
            if (results[i] > -1)
            {
                return true;
            }
        }
        return false;
    }

    [Button]
    void GetXYZ()
    {
        Vector3Int xyz = Vector3Int.zero;
        xyz.z = (int)Math.Floor((float)K / (float)(_height * _width));
        xyz.x = (K - xyz.z * _height * _width) % _height;
        xyz.y = (int)Mathf.Floor((K - xyz.z * _height * _width) / _height);
        Debug.Log(xyz);
    }
}
