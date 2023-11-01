using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Measurer : Tool
{
    [SerializeField] private BaseColliderPointSelector _pSelector;
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Transform _can;

    private Vector2 _lineWidth;

    private GameObject _point1, _point2;

    private void OnEnable()
    {
        _pSelector.OnSelectedNewPoint += SelectNewPoint;
    }

    private void OnDisable()
    {
        _pSelector.OnSelectedNewPoint -= SelectNewPoint;
    }

    private void Start()
    {
        _lineWidth.x = _line.startWidth;
        _lineWidth.y = _line.endWidth;
    }

    private void Update()
    {
        _can.transform.LookAt(Camera.main.transform.position);
    }

    void SelectNewPoint(RaycastHit hit)
    {
        if(_point1 && _point2)
        {
            Destroy(_point1);
            Destroy(_point2);
            _point1 = null;
            _point2 = null;
        }

        if(!_point1)
        {
            HideLine();
            HideText();
            _point1 = Instantiate(_pointPrefab);
            _point1.transform.position = hit.collider.transform.position;
        }
        else
        {
            _point2 = Instantiate(_pointPrefab);
            _point2.transform.position = hit.collider.transform.position;
            DrawLine();
            DrawText(Vector3.Distance(_point1.transform.position, _point2.transform.position).ToString());
        }
    }

    void DrawLine()
    {
        _line.SetWidth(_lineWidth.x,_lineWidth.y);
        _line.SetPosition(0, _point1.transform.position);
        _line.SetPosition(1, _point2.transform.position);
    }

    void HideLine()
    {
        _line.SetWidth(0, 0);
    }

    void DrawText(string text)
    {
        _can.transform.position = _point1.transform.position + (_point2.transform.position - _point1.transform.position) / 2;
        _text.text = text;
    }

    void HideText()
    {
        _text.text = "";
    }
}
