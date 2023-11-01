using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class JsonlPosesParser : PosesParser
{

    private string[] _allLines;

    public JsonlPosesParser(string path, Vector3 offset) : base(path, offset) { }

    public override void Init(int startPointer = 0)
    {
        _pointer = startPointer;
        _allLines = File.ReadAllLines(_path);
    }

    public override void GetValues(int p, out Vector3 pos, out Quaternion rot)
    {
        JObject data = JObject.Parse(_allLines[p]);
        Matrix4x4 matrix = GetMatrixFromData(data);


        pos = Vector3.zero; rot = Quaternion.identity;
        
    }

    Matrix4x4 GetMatrixFromData(JObject data)
    {
        Matrix4x4 matrix = new Matrix4x4();

        for(int i = 0; i < 4; i++)
        {
            Vector4 vec4 = new Vector4((float)data["poseCToW"][i][0], (float)data["poseCToW"][i][1], (float)data["poseCToW"][i][2], (float)data["poseCToW"][i][3]);
            matrix.SetRow(i, vec4);
        }

        return matrix;
    }
}
