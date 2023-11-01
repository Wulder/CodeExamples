using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine.UIElements;

public class RealTimeReader : FrameReader
{
    [SerializeField] private string _path;
    [SerializeField] private int _startPointer;
    [SerializeField] private float _intrinsicDevider;
    

    private string _imagePrefix = "image";
    private string _confidencePrefix = "confidence";
    private string _depthPrefix = "depth";
    private string _dataPrefix = "data";

    private int _pointer;


    #region readData

    void ReadIntrinsics()
    {
        while(!File.Exists($@"{_path}\{_dataPrefix}{_pointer}.json")) { }

        JObject data = JObject.Parse(File.ReadAllText($@"{_path}\{_dataPrefix}{_pointer}.json"));
        _fx = float.Parse(data["calibration_data"]["intrinsic_matrix"][0][0].ToString())/_intrinsicDevider;
        _fy = float.Parse(data["calibration_data"]["intrinsic_matrix"][1][1].ToString())/_intrinsicDevider;
        _cx = float.Parse(data["calibration_data"]["intrinsic_matrix"][2][0].ToString())/_intrinsicDevider;
        _cy = float.Parse(data["calibration_data"]["intrinsic_matrix"][2][1].ToString())/_intrinsicDevider;

    }

    void ReadPoses(out Vector3 position, out Quaternion rotation)
    {


        JObject data = JObject.Parse(File.ReadAllText($@"{_path}\{_dataPrefix}{_pointer}.json"));

        float x = float.Parse(data["calibration_data"]["camera_position"][0].ToString());
        float y = float.Parse(data["calibration_data"]["camera_position"][1].ToString());
        float z = float.Parse(data["calibration_data"]["camera_position"][2].ToString());

        float rotX= float.Parse(data["calibration_data"]["rotation"]["x"].ToString());
        float rotY = float.Parse(data["calibration_data"]["rotation"]["y"].ToString());
        float rotZ = float.Parse(data["calibration_data"]["rotation"]["z"].ToString());
        


        position = new Vector3(x,y,z);
        //rotation = Quaternion.Euler(rotX, rotY, rotZ);
        rotation = Quaternion.EulerAngles(rotX, rotY, rotZ);

        
    }

    
    UnityEngine.Color[] ReadColorImage(string path)
    {
        int pixelsCount = 640 * 480;
        UnityEngine.Color[] colors = new UnityEngine.Color[pixelsCount];
        Bitmap bmp = new Bitmap(path);
        Bitmap resized = new Bitmap(bmp, new System.Drawing.Size(640, 480));

        for (int i = 0; i < pixelsCount; i++)
        {

            var pixel = resized.GetPixel((i % 640), i / 640);
            colors[i].r = (float)pixel.R / 255;
            colors[i].g = (float)pixel.G / 255;
            colors[i].b = (float)pixel.B / 255;

        }

        return colors;
    }
    uint[] ReadDepthImage(string path)
    {
        Mat mat = Cv2.ImRead(path, ImreadModes.AnyDepth);


        ushort[] pixels = new ushort[256 * 192];
        mat.GetArray<ushort>(out pixels);
        uint[] depth = new uint[256 * 192];
        for (int i = 0; i < pixels.Length; i++)
        {
            depth[i] = Convert.ToUInt32(pixels[i]);
        }

        return depth;

    }
    int[] ReadConfidenceImage(string path)
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
    protected override DataFrame CreateData()
    {
        DataFrame data = new DataFrame();

        ReadIntrinsics();
        data.Color = ReadColorImage($@"{_path}\{_imagePrefix}{_pointer}.jpg");
        data.Depth = ReadDepthImage($@"{_path}\{_depthPrefix}{_pointer}.png");
        data.Confidence = ReadConfidenceImage($@"{_path}\{_confidencePrefix}{_pointer}.jpg");
        ReadPoses(out data.Position, out data.Rotation);
        
        
        _pointer++;
        return data;
    }

    protected override void Init()
    {
        base.Init();
        _pointer = _startPointer;
        //ReadIntrinsics();
    }
    #endregion

}
