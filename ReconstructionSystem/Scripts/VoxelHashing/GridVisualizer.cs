using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private Vector3Int _startPos;
    [SerializeField] private Vector3Int _size;
    [SerializeField] private Color _color;

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        for(int i = 0; i < _size.x; i++)
        {
            for(int j = 0; j < _size.y; j++)
            {
                for(int k = 0; k < _size.z; k++)
                {
                    Gizmos.DrawWireCube(new Vector3(i,j,k) + _startPos + Vector3.one / 2, Vector3.one);
                }
            }
        }
    }


}
