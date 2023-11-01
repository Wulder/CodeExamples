using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UiStatistic : MonoBehaviour
{
    [SerializeField] private VoxelReconstruction _reconstructionSystem;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _pointsCount;
    [SerializeField] private TextMeshProUGUI _occupiedPointsCount;
    [SerializeField] private TextMeshProUGUI _chunksCount;
    [SerializeField] private TextMeshProUGUI _gSubBuffersCount;
    [SerializeField] private TextMeshProUGUI _frameNumber;
    [SerializeField] private TextMeshProUGUI _saveProgress;


    private void OnEnable()
    {
        _reconstructionSystem.UpdatePoints += UpdateStats;
        _reconstructionSystem.OnStartSave += UpdateProgressBar;
    }

    private void OnDisable()
    {
        _reconstructionSystem.UpdatePoints -= UpdateStats;
        _reconstructionSystem.OnStartSave -= UpdateProgressBar;
    }
    void Start()
    {
        
    }

    void UpdateProgressBar(ReconstructionSaver saver)
    {
        StartCoroutine(UpdateProgressCoroutine(saver));
    }

    IEnumerator UpdateProgressCoroutine(ReconstructionSaver saver)
    {
        
        while(saver.SaveProgress < 1)
        {
            _saveProgress.text = $"{(int)(saver.SaveProgress*100)}%";
            yield return null;
        }
    }

    void UpdateStats()
    {
        _pointsCount.text = _reconstructionSystem.ReconstructionInfo.TotalPoints.ToString();
        _occupiedPointsCount.text = _reconstructionSystem.ReconstructionInfo.OccupiedPoints.ToString();
        _chunksCount.text = _reconstructionSystem.ReconstructionInfo.ChunksCount.ToString();
        _gSubBuffersCount.text = _reconstructionSystem.ReconstructionInfo.PointBuffer.SubBuffers.Count.ToString();
        _frameNumber.text = _reconstructionSystem.FrameNumber.ToString();
        
    }
}
