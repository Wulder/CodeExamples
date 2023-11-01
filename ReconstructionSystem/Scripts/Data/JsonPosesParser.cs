using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;


public class JsonPosesParser : PosesParser
{
    private JObject _data;
    public JsonPosesParser(string path, Vector3 offset) : base(path, offset) { }

    public override void Init(int startPointer = 0)
    {
        _pointer = startPointer;
        _data = JObject.Parse(File.ReadAllText($@"{_path}"));
        
    }

    public override void GetValues(int p, out Vector3 pos, out Quaternion rot)
    {
        pos = Vector3.zero; rot = Quaternion.identity;
        Debug.Log(_data[p]);
    }
}
