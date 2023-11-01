using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThickWallsGenerator : WallMeshGenerator
{
    private WallsPlan _plan;
    private float _thickness = 0.1f;
    public ThickWallsGenerator(float defaultHeight, Material mat, WallsPlan plan, WallsCreator wallsCreator) : base(defaultHeight, mat, wallsCreator)
    {
        _plan = plan;
    }

    public override Dictionary<Wall, GameObject> BuildWals(List<Wall> walls)
    {
        throw new System.NotImplementedException();
    }

    public override GameObject GenerateWall(Wall wall, float height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];

        //base wall
        Vector3 normal = WallsCreator.GetWallNormal(wall);

        vertices[0] = wall.Point1.Position;
        vertices[1] = wall.Point1.Position + new Vector3(0, height, 0);
        vertices[2] = wall.Point2.Position + new Vector3(0, height, 0);
        vertices[3] = wall.Point2.Position;

        //thickness
        Wall previousWall = wall.Point1._includedWalls.Find(w => w.Point2 == wall.Point1);
        Wall nextWall = wall.Point2._includedWalls.Find(w => w.Point1 == wall.Point2);

        Vector3 corner1 = normal, corner2 = normal;
        float c1Angle = 1, c2Angle = 1;

        if (wall.Point1._includedWalls.Exists(w => w.Point2 == wall.Point1))
        {
            corner1 = WallsCreator.GetCornerNormal(previousWall, wall, out c1Angle);
            
        }

        if (wall.Point2._includedWalls.Exists(w => w.Point1 == wall.Point2))
        {
            corner2 = WallsCreator.GetCornerNormal(nextWall, wall, out c2Angle);
          
        }

        float c1Mod = Mathf.Clamp((Mathf.Abs(c1Angle / 180) * Mathf.PI),1,Mathf.PI);
        float c2Mod = Mathf.Clamp((Mathf.Abs(c2Angle / 180) * Mathf.PI),1,Mathf.PI);

        vertices[4] = wall.Point1.Position + corner1 * (c1Mod) * _thickness;
        vertices[5] = wall.Point1.Position + corner1 * (c1Mod) * _thickness + new Vector3(0, height, 0);
        vertices[6] = wall.Point2.Position + corner2 * (c2Mod) * _thickness + new Vector3(0, height, 0);
        vertices[7] = wall.Point2.Position + corner2 * (c2Mod) * _thickness;

        int[] tris = new int[12]
        {
            2,1,0,
            0,3,2,
            4,5,6,
            6,7,4
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= wall.Point1.Position;
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        GameObject wallObject = new GameObject($"wall");
        wallObject.transform.position = wall.Point1.Position;
        wallObject.AddComponent<MeshFilter>().mesh = mesh;
        wallObject.AddComponent<MeshRenderer>().material = _wallMaterial;

        return wallObject;
    }

    public override void RebuildWall(Wall wall, ref Dictionary<Wall, GameObject> walls)
    {
        if (wall.Point1._includedWalls.Exists(w => w.Point2 == wall.Point1))
        {
            Wall w = wall.Point1._includedWalls.Find(w => w.Point2 == wall.Point1);
            base.RebuildWall(w, ref walls);
        }

        base.RebuildWall(wall, ref walls);

        if (wall.Point2._includedWalls.Exists(w => w.Point1 == wall.Point2))
        {
            Wall w = wall.Point2._includedWalls.Find(w => w.Point1 == wall.Point2);
            base.RebuildWall(w, ref walls);
        }


    }


}
