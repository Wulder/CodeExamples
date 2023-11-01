using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using UnityEngine;
using System.IO;


public class MagicFrameReader : FrameReader
{

    public int PixelsCount { get { return _imageHeight * _imageWidth; } private set { } }

    private string _colorDir = "color", _depthDir = "depth", _confidenceDir = "confidence", _intrisicsDir = "camera_matrix";
    [SerializeField] private string _path;
    [SerializeField] private int _imageWidth, _imageHeight;
    [SerializeField] private int _intrinsicsDevider = 3;
    [SerializeField] private bool _saveDepthMap, _isOtherDepthResolution;
    [SerializeField] private string _saveDepthPath;
    [SerializeField] private int _pointerStart = 0;


    private PosesParser _posesParser;
    private string[] _colorImageFiles;
    private string[] _depthImageFiles;
    private string[] _confidenceImageFiles;
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

    void GetConfidenceImages()
    {
        string path = $"{_path}\\{_confidenceDir}";
        _confidenceImageFiles = Directory.GetFiles(@path);
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
    public virtual uint[] ReadDepthImage(string path)
    {

        Mat mat = Cv2.ImRead(path, ImreadModes.AnyDepth);

        if (_isOtherDepthResolution)
        {
            Cv2.Resize(mat, mat, new OpenCvSharp.Size(256, 192));
        }

        short[] pixels = new short[256 * 192];
        mat.GetArray<short>(out pixels);
        uint[] depth = new uint[256 * 192];

        for (int i = 0; i < pixels.Length; i++)
        {
            depth[i] = Convert.ToUInt32(pixels[i]);
        }

        return depth;
    }
    public virtual int[] ReadConfidenceImage(string path)
    {

        Mat mat = Cv2.ImRead(path, ImreadModes.AnyDepth);

        byte[] pixels = new byte[256 * 192];
        mat.GetArray<byte>(out pixels);
        int[] confidence = new int[256 * 192];

        Cv2.Resize(mat, mat, new OpenCvSharp.Size(640, 480), interpolation: InterpolationFlags.Nearest);

        for (int i = 0; i < pixels.Length; i++)
        {
            confidence[i] = Convert.ToInt32(pixels[i]);
        }


        return confidence;
    }

    #endregion

    protected override void Init()
    {
        _path = Menu.ImagesPath != null ? Menu.ImagesPath : _path;
        GetCoIorImages();
        GetDepthImages();
        GetConfidenceImages();
        ReadIntrinsics();
        _posesLines = File.ReadAllLines($"{_path}\\poses.txt");
        _pointer = _pointerStart;

    }

    private string[] _posesLines;
    protected override DataFrame CreateData()
    {
        DataFrame data = new DataFrame();



        string[] parts = _posesLines[_pointer].Split(',');


        float qx = PosesParser.S2F(parts[5]);
        float qy = PosesParser.S2F(parts[6]);
        float qz = PosesParser.S2F(parts[7]);
        float qw = PosesParser.S2F(parts[8]);

        

        Vector3 pos = new Vector3(PosesParser.S2F(parts[2]), PosesParser.S2F(parts[4]), -PosesParser.S2F(parts[3]));

        //Quaternion rot = new Quaternion(qx* offset.x, qy* offset.y, qz*offset.z, qw* offset.w);
        
        Quaternion rot = new Quaternion(qx, qy, qz, qw);
        //Vector3 eulers = new Vector3(qx, qy, qz);
        

        int _imagesPointer = (int)PosesParser.S2F(parts[1]);

        data.Color = ReadColorImage(_colorImageFiles[_imagesPointer]);
        data.Depth = ReadDepthImage(_depthImageFiles[_imagesPointer]);
        data.Confidence = ReadConfidenceImage(_confidenceImageFiles[_imagesPointer]);

        data.Position = pos;

        data.Rotation = rot;

        

        _pointer++;
        return data;
    }
}
