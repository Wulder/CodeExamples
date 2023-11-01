using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanHandlePipeline : MonoBehaviour
{
    [SerializeField] private FloorAligner _floorAligner;
    [SerializeField] private CornerAligner _cornerAligner;

    [SerializeField] private Button _floorAlignerButton, _cornerAlignButton;
    [SerializeField] private List<Button> _otherButtons = new List<Button>();

    void OnEnable()
    {
        _floorAligner.OnCofirmed += SetActiveCornerAlignButton;
        _cornerAligner.OnConfirmed += SetActiveOtherButtons;
    }
    void OnDisable()
    {
        _floorAligner.OnCofirmed -= SetActiveCornerAlignButton;
        _cornerAligner.OnConfirmed -= SetActiveOtherButtons;
    }


    void Update()
    {

    }

    void SetActiveCornerAlignButton()
    {
        _cornerAlignButton.interactable = true;
    }
    void SetActiveOtherButtons()
    {
        foreach (var button in _otherButtons) { button.interactable = true; }
    }
}