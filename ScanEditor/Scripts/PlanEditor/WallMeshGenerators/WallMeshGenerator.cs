using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WallMeshGenerator 
{


    protected float _wallHeight = 1;
    protected Material _wallMaterial;
    protected WallsCreator _wallsCreator;

    public WallMeshGenerator(float defaultHeight, Material mat, WallsCreator wallsCreator)
    {
        _wallHeight = defaultHeight;
        _wallMaterial = mat;
        _wallsCreator = wallsCreator;
    }

    public abstract Dictionary<Wall, GameObject> BuildWals(List<Wall> walls);
    public abstract GameObject GenerateWall(Wall wall, float height);
    public virtual GameObject GenerateWall(Wall wall) { return GenerateWall(wall, _wallHeight); }
    public virtual void RebuildWall(Wall wall, ref Dictionary<Wall, GameObject> walls)
    {
        GameObject.Destroy(walls[wall]);
        walls[wall] = GenerateWall(wall);
        walls[wall].transform.parent = _wallsCreator.transform;
    }
}
