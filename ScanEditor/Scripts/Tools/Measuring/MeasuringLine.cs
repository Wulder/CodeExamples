using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeasuringLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Vector3 _canvasOffset;
    public Canvas Canvas => _canvas;
    
    void Update()
    {
        _canvas.transform.LookAt(Camera.main.transform);
    }

    public void SetPoints(Vector3 p0, Vector3 p1)
    {
        _lineRenderer.SetPosition(0, p0);
        _lineRenderer.SetPosition(1, p1);
        _text.text = Vector3.Distance(p0, p1).ToString("0.00");
        _canvas.transform.position += _canvasOffset;
    }
}
