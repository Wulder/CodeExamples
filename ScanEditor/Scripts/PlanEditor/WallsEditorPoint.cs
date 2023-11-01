using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsEditorPoint : MonoBehaviour
{

    private WallsPlan _plan;

    [SerializeField] private MeshRenderer _meshRender;

    private Material _material;

    public List<Wall> _includedWalls { get; private set; } = new List<Wall>();
    public Vector3 Position { get { return transform.position; } private set { } }

    private void Awake()
    {
        _material = _meshRender.material;
    }
    public void Init(WallsPlan plan)
    {
        _plan = plan;
    }
    public void IncludeWall(Wall wall)
    {
        _includedWalls.Add(wall);
    }
    public void RemoveIncludeWall(Wall wall)
    {
        _includedWalls.Remove(wall);
    }
    public void SetColor(Color col)
    {
        _material.color = col;
    }

    
}
