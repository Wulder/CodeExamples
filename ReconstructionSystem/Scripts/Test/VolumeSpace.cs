using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSpace : MonoBehaviour
{
    [SerializeField] private int _width, _height, _depth;

    public int[] Elements;

    private void Awake()
    {
        Elements = new int[_width * _height * _depth];
        Array.Fill(Elements, -1);
    }



    private void OnDrawGizmos()
    {
        for(int K = 0; K < Elements.Length; K++)
        {
            Vector3Int xyz = Vector3Int.zero;
            xyz.z = (int)Math.Floor((float)K / (float)(_height * _width));
            xyz.y = (K - xyz.z * _height * _width) % _height;
            xyz.x = (int)Mathf.Floor((K - xyz.z * _height * _width) / _height);

            DrawCube(xyz, Vector3.one);
        }
    }

    void DrawCube(Vector3 pos, Vector3 size)
    {
        Gizmos.DrawWireCube(pos + size/2, size);
    }
}
