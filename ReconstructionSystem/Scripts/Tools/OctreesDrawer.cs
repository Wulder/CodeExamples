using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreesDrawer : Tool
{
    private bool _drawing;
    [SerializeField] private List<Octree> _octrees = new List<Octree>();

    [ContextMenu("draw")]
    public void Draw()
    {
        FillOctrees();
        _drawing = true;
    }

    [ContextMenu("clear")]
    public void Clear()
    {
        _drawing = false;
        _octrees.Clear();
    }

    void FillOctrees()
    {
        for(int i = 0; i < _recInfo._hashMap.Length; i++)
        {
            if(_recInfo._hashMap[i] != -1)
            {
                int index = _recInfo._hashMap[i];
                Octree oct = new Octree();
                oct.end = false;
                oct.pos = VoxelReconstruction.GetPosByIndex(i, _recInfo.RootSize, _recInfo.RootSize);
                oct.branches = new List<Octree>();

                for(int j = 0; j < 8; j++)
                {
                    
                    if (_recInfo._lvls[0][index + j] != -1)
                    {
                        Octree subOct = new Octree();
                        subOct.pos = VoxelReconstruction.GetVecByIndex(j);
                        subOct.end = true;
                        oct.branches.Add(subOct);
                    }
                    
                   
                }

                _octrees.Add(oct);
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        if(_drawing)
        {
            foreach(Octree o in _octrees)
            {
                Gizmos.color = Color.blue;
                //Gizmos.DrawWireCube(o.pos + Vector3.one / 2, Vector3.one);
                DrawOctree(o,Vector3.zero,Vector3.one, 0);
            }
        }
    }

    private void DrawOctree(Octree o, Vector3 pos, Vector3 size, int depth)
    {
        Gizmos.DrawWireCube(pos + (o.pos/Mathf.Pow(2,depth)) + size / 2, size);
        if(!o.end)
        {
            Gizmos.color = Color.red;
          
            foreach(Octree subOct in o.branches)
            {
               // DrawOctree(subOct, pos + o.pos +  size / 2, size / 2, depth+1);
                Gizmos.DrawWireCube(pos + o.pos + subOct.pos/2 + size/4, size/2);
            }
        }
        
        
    }
}

[System.Serializable]
struct Octree
{
    public bool end;
    public Vector3 pos;
    public List<Octree> branches;
}
