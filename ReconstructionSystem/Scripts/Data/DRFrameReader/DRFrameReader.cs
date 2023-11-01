using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using UnityEngine;
using System.IO;

public class DRFrameReader : FrameReader
{
    public int PixelsCount { get { return _imageHeight * _imageWidth; } private set { } }

    private string _colorDir = "color", _depthDir = "depth", _confidenceDir = "confidence", _intrisicsDir = "camera_matrix", _depthExtension = "depthData";
    [SerializeField] private string _path;
    [SerializeField] private int _imageWidth, _imageHeight;
    [SerializeField] private int _intrinsicsDevider = 3;
    [SerializeField] private bool _saveDepthMap, _isOtherDepthResolution;
    [SerializeField] private string _saveDepthPath;
    [SerializeField] private int _pointerStart = 0;
    [SerializeField] private int _depthScale;


    private PosesParser _posesParser;
    private string[] _colorImageFiles;
    private string[] _depthImageFiles;
    private int _pointer;




    #region LoadFileNames
    void GetDepthImages()
    {
        string path = $"{_path}\\{_depthDir}";
        _depthImageFiles = Directory.GetFiles(@path);
    }

    void GetCoIorImages()
    {
        string path = $"{_path}\\{_colorDir}";
        _colorImageFiles = Directory.GetFiles(@path);
    }

    
    #endregion

    #region ReadData
    public virtual void ReadIntrinsics()
    {


        string[] lines = File.ReadAllLines($"{_path}\\{_intrisicsDir}.csv");

        string sp = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;

        _fx = float.Parse(lines[0].Split(',')[0].Replace(".", sp)) / _intrinsicsDevider;
        _fy = float.Parse(lines[1].Split(',')[1].Replace(".", sp)) / _intrinsicsDevider;


        _cx = float.Parse(lines[0].Split(',')[2].Replace(".", sp)) / _intrinsicsDevider;
        _cy = float.Parse(lines[1].Split(',')[2].Replace(".", sp)) / _intrinsicsDevider;


    }
    public virtual UnityEngine.Color[] ReadColorImage(string path)
    {
        UnityEngine.Color[] colors = new UnityEngine.Color[PixelsCount];
        Bitmap bmp = new Bitmap(path);

        for (int i = 0; i < PixelsCount; i++)
        {

            var pixel = bmp.GetPixel((i % _imageWidth), i / _imageWidth);
            colors[i].r = (float)pixel.R / 255;
            colors[i].g = (float)pixel.G / 255;
            colors[i].b = (float)pixel.B / 255;

        }

        return colors;
    }
    public virtual uint[] ReadDepthData(string path)
    {

        
        uint[] depth = new uint[256 * 192];

        BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open,FileAccess.Read));

        for (int i = 0; i < depth.Length; i++)
        {
            float f = br.ReadSingle()/5.0f;
            
                
            depth[i] = (uint)(f * _depthScale);
        }
        br.Close();
        
        return depth;
    }
    public virtual int[] ReadConfidenceImage()
    {

        int[] data = new int[256 * 192];
        Array.Fill(data, 10);


        return data;
    }

    #endregion

    protected override void Init()
    {
        _path = Menu.ImagesPath != null ? Menu.ImagesPath : _path;
        GetCoIorImages();
        GetDepthImages();
        ReadIntrinsics();
        _posesLines = File.ReadAllLines($"{_path}\\poses.txt");
        _pointer = _pointerStart;

    }

    private string[] _posesLines;
    [SerializeField] float W;
    protected override DataFrame CreateData()
    {
        DataFrame data = new DataFrame();



        string[] parts = _posesLines[_pointer].Split(',');


        float qx = PosesParser.S2F(parts[4]);
        float qy = PosesParser.S2F(parts[5]);
        float qz = PosesParser.S2F(parts[6]);
        float qw = PosesParser.S2F(parts[7]);



        Vector3 pos = new Vector3(PosesParser.S2F(parts[1]), PosesParser.S2F(parts[2]), PosesParser.S2F(parts[3]));

        //Quaternion rot = new Quaternion(qx* offset.x, qy* offset.y, qz*offset.z, qw* offset.w);

        Quaternion rot = new Quaternion(qx, qy, qz, qw);
        //Vector3 eulers = new Vector3(qx, qy, qz);


       

        data.Color = ReadColorImage(_colorImageFiles[_pointer]);
        data.Depth = ReadDepthData(_depthImageFiles[_pointer]);
        data.Confidence = ReadConfidenceImage();

        data.Position = pos;

        data.Rotation = rot;



        _pointer++;
        return data;
    }
}
