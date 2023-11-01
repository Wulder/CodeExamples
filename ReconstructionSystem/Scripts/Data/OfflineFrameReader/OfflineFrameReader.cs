using UnityEngine;
using System.IO;
using System.Drawing;
using OpenCvSharp;
using System;
using System.Globalization;

public class OfflineFrameReader : FrameReader
{
    public int PixelsCount { get { return _imageHeight * _imageWidth; } private set { } }

    private string _colorDir = "color", _depthDir = "depth", _confidenceDir = "confidence", _intrisicsDir = "camera_matrix";
    [SerializeField] private string _path, _posesFileName = "poses.txt";
    [SerializeField] private PosesReader _posesReaderVariant;
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


    #region overrides
    protected override void Init()
    {
        _path = Menu.ImagesPath != null ? Menu.ImagesPath : _path;
        GetCoIorImages();
        GetDepthImages();
        GetConfidenceImages();
        ReadIntrinsics();
        _pointer = _pointerStart;

        CreatePosesParser();
        _posesParser.Init(_pointer);
    }

    void CreatePosesParser()
    {
        switch (_posesReaderVariant)
        {
            case PosesReader.DefaultTextReader:
                {
                    _posesParser = new PosesParser($"{_path}\\{_posesFileName}", Vector3.zero);
                    break;
                }
            case PosesReader.JsonReader:
                {
                    _posesParser = new JsonPosesParser($"{_path}\\{_posesFileName}", Vector3.zero);
                    break;
                }
            default:
                {
                    _posesParser = new PosesParser($"{_path}\\{_posesFileName}", Vector3.zero);
                    break;
                }
        }
    }

    override protected DataFrame CreateData()
    {
        DataFrame data = new DataFrame();
        data.Color = ReadColorImage(_colorImageFiles[_pointer]);
        data.Depth = ReadDepthImage(_depthImageFiles[_pointer]);
        data.Confidence = ReadConfidenceImage(_confidenceImageFiles[_pointer]);

        _posesParser.GetValues(_pointer, out data.Position, out data.Rotation);

        _pointer++;
        return data;

    }
    #endregion

    enum PosesReader
    {
        DefaultTextReader,
        JsonReader
    }
}
