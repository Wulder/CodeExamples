
using UnityEngine;
using System.IO;
using System.Globalization;

public class PosesParser 
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    private protected string _path;
    private protected  Vector3 _offset;
    private protected int _pointer = 0;

    private string[] _posesLines;
    public PosesParser(string path, Vector3 offset)
    {
        _path = path;
        _offset = offset;
    }

    public virtual void Init(int startPointer = 0)
    {
        _posesLines = File.ReadAllLines(_path);
        _pointer = startPointer;
    }

    

    public virtual void GetValues(int p, out Vector3 pos, out Quaternion rot)
    {
     
        string[] parts = _posesLines[p].Split(',');
        pos = new Vector3(S2F(parts[1]), S2F(parts[2]), S2F(parts[3])) + _offset;
        rot = new Quaternion(S2F(parts[4]), S2F(parts[5]), S2F(parts[6]), S2F(parts[7]));
        

    }

    public static float S2F(string line)
    {
        float res;
        try
        {
            string sp = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            line = line.Replace(".", sp);
            res = float.Parse(line);
        }
        catch
        {
            res = 0f;
        }

        return res;
    }

}


