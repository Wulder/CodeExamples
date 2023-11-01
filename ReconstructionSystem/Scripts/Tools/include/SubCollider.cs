using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCollider : MonoBehaviour
{
    private int _lvl;
    private int _index;
    private Vector3 _pos;

    public int Lvl => _lvl;
    public int LvlIndex => _index;
    public Vector3 Pos => _pos;

    public void Init(int lvl, int index, Vector3 pos)
    {
        _lvl = lvl;
        _index = index;
        _pos = pos;
    }

   
    
}
