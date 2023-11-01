using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class RectangleBinarySaver : IRectangleSerializer, IRectangleDeserializer
{

    private string _path;
    public RectangleBinarySaver(string path)
    {
        _path = path;
    }

    public void SetPath(string path)
    {
        _path = path;
    }

    public List<Rectangle> Deserialize()
    {
        List<Rectangle> result = new List<Rectangle>();

        FileStream fs = File.Open(_path, FileMode.Open);
        BinaryReader reader = new BinaryReader(fs);

        int count = reader.ReadInt32();

        for(int i = 0; i < count; i++)
        {
            result.Add(DeserializeRectangle(reader));
        }

        Debug.Log("Success deserialize!");

        fs.Close();
        reader.Close();
        return result;
    }

    public void Serialize(List<Rectangle> rectangles)
    {
        FileStream fs = File.Create(_path);
        BinaryWriter bw = new BinaryWriter(fs);

        bw.Write(rectangles.Count);

        foreach (Rectangle rect in rectangles)
        {
            SerializeRectangle(bw, rect);
        }
        bw.Close();
        Debug.Log($"Success save! ({_path})");
    }

    private void SerializeRectangle(BinaryWriter bw, Rectangle rect)
    {
        #region write points
        bw.Write(rect.p1.x);
        bw.Write(rect.p1.y);
        bw.Write(rect.p1.z);

        bw.Write(rect.p2.x);
        bw.Write(rect.p2.y);
        bw.Write(rect.p2.z);
        #endregion

        #region writeHoles
        bw.Write(rect.Holes.Count); //holes count

        foreach(var hole in rect.Holes)
        {
            SerializeHole(bw, hole);
        }
        #endregion

    }

    Rectangle DeserializeRectangle(BinaryReader reader)
    {
        Vector3 p1 = new Vector3();
        Vector3 p2 = new Vector3();

        p1.x = reader.ReadSingle();
        p1.y = reader.ReadSingle();
        p1.z = reader.ReadSingle();

        p2.x = reader.ReadSingle();
        p2.y = reader.ReadSingle();
        p2.z = reader.ReadSingle();

        Rectangle rect = new Rectangle(p1, p2);

        int holesCount = reader.ReadInt32();
      
        for(int i = 0; i < holesCount; i++)
        {
            rect.Holes.Add(DeserializeHole(reader));
        }

        return rect;
    }

    private void SerializeHole(BinaryWriter bw, RectangleHole hole)
    {
        bw.Write(hole.Normal.x);
        bw.Write(hole.Normal.y);
        bw.Write(hole.Normal.z);

        bw.Write(hole.Size.x);
        bw.Write(hole.Size.y);

        bw.Write(hole.Position.x);
        bw.Write(hole.Position.y);
    }

    private RectangleHole DeserializeHole(BinaryReader br)
    {
        RectangleHole hole = new RectangleHole();

        hole.Normal.x = br.ReadSingle();
        hole.Normal.y = br.ReadSingle();
        hole.Normal.z = br.ReadSingle();

        hole.Size.x = br.ReadSingle();
        hole.Size.y = br.ReadSingle();

        hole.Position.x = br.ReadSingle();
        hole.Position.y = br.ReadSingle();


        return hole;
    }
}
