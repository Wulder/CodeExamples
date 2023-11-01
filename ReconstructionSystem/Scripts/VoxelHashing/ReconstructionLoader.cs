using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.VFX;
using System;

public class ReconstructionLoader : MonoBehaviour
{
    public event Action OnLoad;
    private ReconstructionSaver _reconstructionSaver;
    private PointBuffer _pointsBuffer;
    private ReconstructionInfo _reconstructionInfo;
    [SerializeField] private VisualEffect _visualizer;
    public string _path;

    private void Start()
    {
        if(Menu.ReconstructionFilePath != null) 
            _path = Menu.ReconstructionFilePath;

        _reconstructionSaver = new ReconstructionSaver();
        Load();
        Draw();
    }


    //[ContextMenu("load")]
    //async void Load()
    //{
    //    await Task.Run(()=> {
    //        Debug.Log("start loading reconstruction");
    //        string path = @"E:\UnityProjects\ReconstructionSystem\Assets\Reconstructions\test1.escan";
    //         _reconstructionSaver.ReadReconstructionData(path);
    //        Debug.Log("data loaded");

    //    });

    //    _reconstructionInfo = _reconstructionSaver.ReconstructionInfo;

    //}

    [ContextMenu("load")]
    void Load()
    {

        Debug.Log("start loading reconstruction");
        string path = $@"{_path}";
        _reconstructionSaver.ReadReconstructionData(path);
        Debug.Log("data loaded");

        _reconstructionInfo = _reconstructionSaver.ReconstructionInfo;
        OnLoad?.Invoke();

    }


    [ContextMenu("Draw")]
    void Draw()
    {
        for (int i = 0; i < _reconstructionSaver.VisualizersInfo.Length; i++)
        {
            PointVisualizer pv = Instantiate(_visualizer).GetComponent<PointVisualizer>();
            PointVisualizerInfo vInfo = _reconstructionSaver.VisualizersInfo[i];
            pv.Init(vInfo.Offset, vInfo.VisualizerNumber, _reconstructionInfo.PointBuffer.SubBuffers[vInfo.GBufferIndex],vInfo.GBufferIndex, vInfo.BoundCenter, vInfo.BoundSize);
           // pv.DecreseByDistance(true);
        }
    }
}
