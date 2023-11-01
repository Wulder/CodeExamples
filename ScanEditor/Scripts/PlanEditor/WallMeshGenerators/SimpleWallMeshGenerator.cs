using System.Collections;
using System.Collections.Generic;
using TriLibCore.Dae.Schema;
using UnityEngine;

public class SimpleWallMeshGenerator : WallMeshGenerator
{
    public SimpleWallMeshGenerator(float wallsHeight, Material mat, WallsCreator wallsCreator) : base(wallsHeight, mat,wallsCreator) { }

    public override Dictionary<Wall, GameObject> BuildWals(List<Wall> walls)
    {
        Dictionary<Wall,GameObject> result = new Dictionary<Wall,GameObject>();


        foreach (Wall wall in walls)
        {
            result[wall] = GenerateWall(wall);
        }


        return result;
    }

    public override GameObject GenerateWall(Wall wall, float height)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];

        vertices[0] = wall.Point1.Position;
        vertices[1] = wall.Point1.Position + new Vector3(0, height, 0);
        vertices[2] = wall.Point2.Position + new Vector3(0, height, 0);
        vertices[3] = wall.Point2.Position;

        vertices[4] = wall.Point1.Position;
        vertices[5] = wall.Point1.Position + new Vector3(0, height, 0);
        vertices[6] = wall.Point2.Position + new Vector3(0, height, 0);
        vertices[7] = wall.Point2.Position;

        int[] tris = new int[12]
        {
            0,1,2,
            2,3,0,
            2,1,0,
            0,3,2
        };

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= wall.Point1.Position;
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;

        GameObject wallObject = new GameObject($"wall");
        wallObject.transform.position = wall.Point1.Position;
        wallObject.AddComponent<MeshFilter>().mesh = mesh;
        wallObject.AddComponent<MeshRenderer>().material = _wallMaterial;

        return wallObject;
    }
    public override GameObject GenerateWall(Wall wall)
    {
        return GenerateWall(wall, _wallHeight);
    }

    

}
