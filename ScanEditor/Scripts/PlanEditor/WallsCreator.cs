using Exoa.Designer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniGLTF;
using UnityEngine;

public class WallsCreator : MonoBehaviour
{
    [SerializeField] private WallsPlan _plan;
    [SerializeField] private float _wallHeight = 1;
    [SerializeField] private Material _wallMaterial;
    [SerializeField] private WallGenerator _wallGenerator;

    // private List<GameObject> _walls = new List<GameObject>();
    private Dictionary<Wall, GameObject> _walls = new Dictionary<Wall, GameObject>();

    private WallMeshGenerator _generator;

    private void Awake()
    {
        switch (_wallGenerator)
        {
            case WallGenerator.SimpleWall:
                {
                    _generator = new SimpleWallMeshGenerator(_wallHeight, _wallMaterial,this);
                    break;
                }
            case WallGenerator.ThickWall:
                {
                    _generator = new ThickWallsGenerator(_wallHeight, _wallMaterial, _plan,this);
                    break;
                }
        }
    }

    private void OnEnable()
    {
        _plan.OnAddWall += CreateWall;
        _plan.OnRemoveWall += DestroyWall;
        _plan.OnPointTranslate += RebuildWall;
    }

    private void OnDisable()
    {
        _plan.OnAddWall -= CreateWall;
        _plan.OnRemoveWall -= DestroyWall;
        _plan.OnPointTranslate -= RebuildWall;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (var w in _plan.Walls)
            {
                BuildWalls();
            }
        }

    }

    void CreateWall(Wall wall)
    {
        var gm = _generator.GenerateWall(wall, _wallHeight);
        gm.transform.SetParent(_plan.transform, false);
        gm.AddComponent<MeshCollider>();
        gm.layer = LayerMask.NameToLayer("Wall");
        _walls[wall] = gm;

        RebuildWall(wall.Point1);
        

    }

    void DestroyWall(Wall wall)
    {
        if (!_walls.ContainsKey(wall) || _walls[wall] == null) return;
        Destroy(_walls[wall]);
        _walls.Remove(wall);
    }

    void RebuildWall(WallsEditorPoint point)
    {
        var walls = point._includedWalls.ToArray();

        for (int i = 0; i < walls.Length; i++)
        {
            _generator.RebuildWall(walls[i], ref _walls);
        }
    }

    void BuildWalls()
    {
        var keys = _walls.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            DestroyWall(keys[i]);
        }
        _walls.Clear();

        //foreach (var w in _plan.Walls)
        //{
        //    CreateWall(w);
        //}

        _walls = _generator.BuildWals(_plan.Walls);

    }

    void ClearWalls()
    {
        var keys = _walls.Keys.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            DestroyWall(keys[i]);
        }
    }

    public static Vector3 GetWallNormal(Wall wall)
    {
        Vector3 normal;
        var A = wall.Point1.Position;
        var B = wall.Point1.Position + new Vector3(0, 1, 0);
        var C = wall.Point2.Position + new Vector3(0, 1, 0);
        normal = Vector3.Cross(B - A, C - A).normalized;
        return normal;
    }

    public static Vector3 GetCornerNormal(Wall w1, Wall w2)
    {
        var n1 = GetWallNormal(w1);
        var n2 = GetWallNormal(w2);
        return (n1 + n2).normalized;
    }

    public static Vector3 GetCornerNormal(Wall w1, Wall w2, out float angle)
    {
        var n1 = GetWallNormal(w1);
        var n2 = GetWallNormal(w2);
        angle = Vector3.Angle(n1, n2);
        return (n1 + n2).normalized;
    }
    private void OnDrawGizmos()
    {
        foreach (var key in _walls.Keys)
        {
            var A = key.Point1.Position;
            var B = key.Point1.Position + new Vector3(0, _wallHeight, 0);
            var C = key.Point2.Position + new Vector3(0, _wallHeight, 0);
            var D = key.Point2.Position;


            Vector3 startPos = A + (D - A) / 2 + new Vector3(0, _wallHeight / 2, 0);

            Gizmos.DrawRay(startPos, GetWallNormal(key));

        }

        Gizmos.color = Color.yellow;

        var keys = _walls.Keys.ToList();

        foreach (var key in keys)
        {
            if (keys.Exists(p => p.Point1 == key.Point2))
            {
                Vector3 normal1 = GetWallNormal(key);
                Wall nextWall = keys.Find(p => p.Point1 == key.Point2);

                Vector3 normal2 = GetWallNormal(nextWall);
                Vector3 direction = (normal1 + normal2).normalized;
                Gizmos.DrawRay(key.Point2.Position, direction);
            }
        }
    }
}

public enum WallGenerator
{
    SimpleWall,
    ThickWall
}