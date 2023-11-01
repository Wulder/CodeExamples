using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashmapDrawer : Tool
{
    private bool _drawing;
    private List<Vector3> _occupiedHashes = new List<Vector3>();

    [ContextMenu("draw")]
    public void Draw()
    {
        FillOccupiedHashes();
        _drawing = true;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        _occupiedHashes.Clear();
        _drawing = false;
    }
    void FillOccupiedHashes()
    {
        for (int i = 0; i < _recInfo._hashMap.Length; i++)
        {
            if (_recInfo._hashMap[i] != -1)
            {
                Vector3 pos = VoxelReconstruction.GetPosByIndex(i, _recInfo.RootSize, _recInfo.RootSize);
                _occupiedHashes.Add(pos);
            }

        }
    }

    private void OnDrawGizmos()
    {
        if(_drawing)
        {
            foreach (Vector3 p in _occupiedHashes)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(p + Vector3.one / 2, Vector3.one);
            }
        }
    }
}
