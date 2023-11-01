using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;
using UnityEngine.Events;
using ImageMagick;
using OpenCvSharp;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;


public class VoxelReconstruction : MonoBehaviour
{
    public event Action<DataFrame> UpdateData;
    public event Action<ReconstructionSaver> OnStartSave;
    public UnityEvent OnSavedDelegate;

    public event Action UpdatePoints;
    public Vector3Int RootSize => new Vector3Int(_rootSize, _rootSize, _rootSize);


    [SerializeField] private int _rootSize;

    [Header("Components")]
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private FrameReader _frameReader;
    [SerializeField] private Transform _followingCamera;


    [Header("Parameters")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _chunksDensity, _octreeDepth, _pointSubBufferSize;
    [SerializeField] private int _minimumConfidence;
    [SerializeField] private float _depthScale;
    [SerializeField] private Vector3 _rotateDataset;

 


    [Header("Prefabs")]
    [SerializeField] private VisualEffect _visualizerPrefab;

    [Header("Filters")]
    [SerializeField] private DepthFilter _filter;

    [SerializeField] private bool _frameToFrame;



    private PointBuffer _pointsBuffer;
    private ComputeBuffer _colors, _depth, _confidence;
    private ComputeBuffer _tempPoints;


    private int _chunkCube;
    private int _totalPoints, _lastBufferPointer;
    private int _visualizerCount;


    private VisualEffect _lastVisualizer;
    private List<PointVisualizer> _visualizers = new List<PointVisualizer>();
    private Vector3 _startBoundPoint, _endBoundPoint;



    public static float[] _octreeSizes = new float[10] { 1, 0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f, 0.015625f, 0.0078125f, 0.00390625f, 0.001953125f };
    public static float[] _vecValues = new float[9] { 0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f, 0.015625f, 0.0078125f, 0.00390625f, 0.001953125f };
    private bool _isBoundsInit = false;
    private int _currentFrame = 0;
    public string _savePath;
    private ReconstructionInfo _reconstructionInfo;

    public ReconstructionInfo ReconstructionInfo => _reconstructionInfo;
    public int FrameNumber => _currentFrame;



    private void OnDisable()
    {

        _pointsBuffer.Release();
        _colors.Release();
        _depth.Release();
        _confidence.Release();
        _tempPoints.Release();



    }
    private void Start()
    {

        if (Menu.ImagesPath != null)
            SetSavePath($"{Menu.ImagesPath}\reconstruction.escan");

        if (_pointSubBufferSize > 0)
            _pointsBuffer = new PointBuffer(_computeShader, _width * _height);
        else
            _pointsBuffer = new PointBuffer();

        resultPoints = new PointData[_width * _height];

        _colors = new ComputeBuffer(_width * _height, Marshal.SizeOf(typeof(Color)));
        _depth = new ComputeBuffer(_width * _height, Marshal.SizeOf(typeof(int)));
        _confidence = new ComputeBuffer(_width * _height, Marshal.SizeOf(typeof(int)));


        _chunkCube = _chunksDensity * _chunksDensity * _chunksDensity;
        _visualizerCount = 0;
        _reconstructionInfo = new ReconstructionInfo(_rootSize, _octreeDepth, _chunksDensity, _pointsBuffer);
    }
    private void CreatePoints(DataFrame frame)
    {
        _tempPoints = new ComputeBuffer(_width * _height, Marshal.SizeOf(typeof(PointData)));
        int kernelIndex = _computeShader.FindKernel("CreatePoint");
        _colors.SetData(frame.Color);
        _depth.SetData(frame.Depth);
        _confidence.SetData(frame.Confidence);


        _computeShader.SetBuffer(kernelIndex, "colors", _colors);
        _computeShader.SetBuffer(kernelIndex, "depth", _depth);
        _computeShader.SetBuffer(kernelIndex, "confidence", _confidence);
        _computeShader.SetBuffer(kernelIndex, "tempPoints", _tempPoints);


        _computeShader.SetInt("rootSize", _rootSize);
        _computeShader.SetInt("voxelBlockSize", _reconstructionInfo.ChunkDensity);
        _computeShader.SetInt("floorValue", _reconstructionInfo.Density);
        _computeShader.SetInt("octreeDepth", _reconstructionInfo.OctreeDepth);
        _computeShader.SetInt("minimumConfidence", _minimumConfidence);

        _computeShader.SetFloat("depthScale", _depthScale);
        _computeShader.SetFloat("fx", _frameReader.Fx);
        _computeShader.SetFloat("fy", _frameReader.Fy);
        _computeShader.SetFloat("cx", _frameReader.Cx);
        _computeShader.SetFloat("cy", _frameReader.Cy);
        _computeShader.SetFloats("cameraPosition", frame.Position.x, frame.Position.y, frame.Position.z);

        _followingCamera.position = frame.Position;
        _followingCamera.rotation = frame.Rotation;
       // _followingCamera.forward = -_followingCamera.forward;
        //!!!! поворот на 90 градусов из-за перевернутого датасета
        _followingCamera.Rotate(_rotateDataset);

        Vector3 camDir = _followingCamera.forward;
        _computeShader.SetFloats("cameraDirection", camDir.x, camDir.y, camDir.z);
        camDir = _followingCamera.right;
        _computeShader.SetFloats("cameraDirectionRight", camDir.x, camDir.y, camDir.z);
        camDir = _followingCamera.up;
        _computeShader.SetFloats("cameraDirectionUp", camDir.x, camDir.y, camDir.z);

        _computeShader.Dispatch(kernelIndex, _width * _height / 1024, 1, 1);




        PointData[] pops = new PointData[_width * _height];
        _tempPoints.GetData(pops);

        PutPoints(pops, frame);
        _tempPoints.Release();

        AddVisualizer();
    }
    void AddVisualizer()
    {
        if (_reconstructionInfo.TotalPoints >= 1000000 * _visualizerCount)
        {
            if (_visualizerCount > 0)
            {
                Vector3 center = _startBoundPoint + ((_endBoundPoint - _startBoundPoint) / 2);

                _lastVisualizer.GetComponent<PointVisualizer>().SetCenter(center);
                _lastVisualizer.GetComponent<PointVisualizer>().SetSize(_endBoundPoint - _startBoundPoint);

                PointVisualizer pointVisualizer = _lastVisualizer.GetComponent<PointVisualizer>();
                pointVisualizer.SetCenter(center);
                pointVisualizer.DecreseByDistance(true);

            }

            VisualEffect ve = Instantiate(_visualizerPrefab);
            ve.GetComponent<PointVisualizer>().Init(1000000 * _visualizerCount, _visualizerCount, _pointsBuffer.SubBuffers[_pointsBuffer.GetSubBuferByPointer(1000000 * _visualizerCount)], _pointsBuffer.GetSubBuferByPointer(1000000 * _visualizerCount), Vector3.zero, Vector3.one * _rootSize * 10);

            _lastVisualizer = ve;
            _visualizers.Add(_lastVisualizer.GetComponent<PointVisualizer>());
            _startBoundPoint = -Vector3.one;
            _endBoundPoint = -Vector3.one;
            _isBoundsInit = false;

            _visualizerCount++;
        }
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) || !_frameToFrame || Input.GetKey(KeyCode.P))
        {
            DataFrame frame;

            if (_frameReader.GetFrame(out frame))
            {

                Mat mat2 = new Mat(192, 256, MatType.CV_16UC1);
                Mat mat3 = new Mat(192, 256, MatType.CV_16UC1);
                ushort[] shortDepth = new ushort[256 * 192];

                for (int i = 0; i < shortDepth.Length; i++)
                {
                    shortDepth[i] = (ushort)frame.Depth[i];
                }

                mat2.SetArray(shortDepth);


                Cv2.Resize(mat2, mat3, new Size(640, 480), interpolation: InterpolationFlags.Nearest);


                if (_filter)
                    _filter.Execute(ref frame.Depth, 256, 192);

                ushort[] modDepth = new ushort[256 * 192];

                for (int i = 0; i < modDepth.Length; i++)
                {
                    modDepth[i] = Convert.ToUInt16(frame.Depth[i]);
                }


                mat2.SetArray(modDepth);


                ushort[] pixels = new ushort[640 * 480];

                Cv2.Resize(mat2, mat3, new Size(640, 480), interpolation: InterpolationFlags.Nearest);
                // mat3.SaveImage(@"C:\Users\simp\Desktop\out\out.png");
                mat3.GetArray(out pixels);
                frame.Depth = new uint[640 * 480];
                for (int i = 0; i < pixels.Length; i++)
                {
                    frame.Depth[i] = Convert.ToUInt32(pixels[i]);
                }

                CreatePoints(frame);
                UpdateData?.Invoke(frame);
              
            }
        }
    }

    PointData[] resultPoints;
    unsafe void PutPoints(PointData[] points, DataFrame frame)
    {
        if (_currentFrame == 0)
        {
            Vector3 pOffset = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Point.Position == Vector3.zero)
                    continue;

                if (pOffset == Vector3.zero)
                    pOffset = points[i].Point.Position;

                if (Camera.main.transform.InverseTransformVector(points[i].Point.Position).y < Camera.main.transform.InverseTransformVector(pOffset).y
                    && Camera.main.transform.InverseTransformVector(points[i].Point.Position).x <= Camera.main.transform.InverseTransformVector(pOffset).x)
                {
                    pOffset = points[i].Point.Position;
                }
            }
            _reconstructionInfo.SetPivotOffset(pOffset - frame.Position);
            _reconstructionInfo.SetPivot(pOffset);
        } //вычисляем пивот реконструкции из первого фрейма

        int hash = 0;


        int lvlIndex = 0;
        int finalIndex = 0;

        int resultPointCount = 0;


        for (int i = 0; i < _width * _height; i++)
        {

            if (points[i].Point.Position == Vector3.zero)
                continue;


            if (!_isBoundsInit)
            {
                _endBoundPoint = points[i].Point.Position;
                _startBoundPoint = points[i].Point.Position;
                _isBoundsInit = true;
            }

            hash = points[i].Hash;

            if (_reconstructionInfo._hashMap[hash] == -1)
            {
                _reconstructionInfo._hashMap[hash] = _reconstructionInfo._pointers[0];
                _reconstructionInfo._pointers[0] += 8;

            }

            #region Dynamic Octree

            finalIndex = _reconstructionInfo._hashMap[hash];

            for (int j = 0; j < _octreeDepth; j++)
            {

                lvlIndex = points[i].lvls[j];


                if (_reconstructionInfo._lvls[j][finalIndex + lvlIndex] == -1)
                {
                    _reconstructionInfo._lvls[j][finalIndex + lvlIndex] = _reconstructionInfo._pointers[j + 1];
                    _reconstructionInfo._pointers[j + 1] += (j < (_octreeDepth - 1)) ? 8 : _chunkCube;
                }
                finalIndex = _reconstructionInfo._lvls[j][finalIndex + lvlIndex];
            }

            points[i].PointIndex += finalIndex;
            resultPoints[resultPointCount] = points[i];
            resultPointCount++;

            CompareBoundPositions(points[i].Point.Position);

            if (finalIndex > _reconstructionInfo.LastPointBufferPointer)
                _reconstructionInfo.SetLastBufferPointer(finalIndex);

            #endregion
        }



        _reconstructionInfo.AddOccupiedPoints(_pointsBuffer.SetData(resultPoints, _width * _height));


        UpdatePoints?.Invoke();
        _currentFrame++;
    }



    void CompareBoundPositions(Vector3 pos)
    {

        //start

        if (_startBoundPoint.x > pos.x)
            _startBoundPoint.x = pos.x;

        if (_startBoundPoint.y > pos.y)
            _startBoundPoint.y = pos.y;

        if (_startBoundPoint.z > pos.z)
            _startBoundPoint.z = pos.z;

        //end

        if (_endBoundPoint.x < pos.x)
            _endBoundPoint.x = pos.x;

        if (_endBoundPoint.y < pos.y)
            _endBoundPoint.y = pos.y;

        if (_endBoundPoint.z < pos.z)
            _endBoundPoint.z = pos.z;
    }
    public static int GetIndexByPos(Vector3Int pos, int size)
    {
        return (pos.x + (pos.y * size) + ((size * size) * pos.z));
    }
    public static int GetIndexByPos(Vector3 pos, int size)
    {
        return (int)(pos.x + (pos.y * size) + ((size * size) * pos.z));
    }
    public static int GetIndexInOctree(Vector3 pos, float size)
    {
        int index = 0;

        index = pos.z < size / 2 ? 0 : 4;
        index += pos.y < size / 2 ? 0 : 2;
        index += pos.x < size / 2 ? 0 : 1;

        return index;
    }
    public static Vector3Int GetVecByIndex(int index)
    {
        Vector3Int vec = Vector3Int.zero;

        if (index == 1 || index == 3 || index == 5 || index == 7)
        {
            vec.x = 1;
        }

        if (index == 2 || index == 3 || index == 6 || index == 7)
        {
            vec.y = 1;
        }

        if (index >= 4)
        {
            vec.z = 1;
        }

        return vec;
    }
    public static Vector3 GetDevidedVecByIndex(int index, int lvl)
    {
        Vector3 result = Vector3.zero;

        if (index == 1 || index == 3 || index == 5 || index == 7)
        {
            result.x = _vecValues[lvl];
        }

        if (index == 2 || index == 3 || index == 6 || index == 7)
        {
            result.y = _vecValues[lvl];
        }

        if (index >= 4)
        {
            result.z = _vecValues[lvl];
        }


        return result;
    }
    public static Vector3Int GetPosByIndex(int K, int h, int w)
    {
        Vector3Int pos = Vector3Int.zero;
        pos.z = (int)Math.Floor((float)K / (float)(h * w));
        pos.x = (K - pos.z * h * w) % h;
        pos.y = (int)Mathf.Floor((K - pos.z * h * w) / h);

        return pos;
    }
    public void StartReconstruct()
    {
        _frameToFrame = false;
    }
    public void Pause()
    {
        _frameToFrame = true;
    }
    public void StartPause()
    {
        _frameToFrame = !_frameToFrame;
    }
    public void SetSavePath(string path)
    {
        _savePath = $@"{path}";
    }
    public void Save()
    {


        ReconstructionSaver data = new ReconstructionSaver(_reconstructionInfo, _visualizers.ToArray());

        data.SaveReconstructionData(_savePath, OnSavedDelegate);
        OnStartSave?.Invoke(data);

    }
}
