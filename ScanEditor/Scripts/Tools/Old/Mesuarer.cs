using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Mesuarer : OldTool
{
    private List<Measurement> _measurements = new List<Measurement>();
    private List<GameObject> _lines = new List<GameObject>();
    [SerializeField] private GameObject _linePrefab;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                
                    SetPoint(hit.point);
                

            }
        }

        foreach (var line in _lines) 
        {
            line.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 25), "Clear"))
        {
            Clear();
        }
    }

    void Clear()
    {
        _measurements.Clear();
        foreach (var line in _lines)
            Destroy(line);

        _lines.Clear();
    }    

    void CreateMeasuringLine(Measurement measurement)
    {
        GameObject go = Instantiate(_linePrefab);
        var lr = go.GetComponent<LineRenderer>();
        var text = go.GetComponentInChildren<TextMeshProUGUI>();


        lr.SetPosition(0, measurement.P1);
        lr.SetPosition(1, measurement.P2);

        text.text = $"{Vector3.Distance(measurement.P1, measurement.P2).ToString("0.00")}m";
        go.transform.position = Vector3.Lerp(measurement.P2, measurement.P1, 0.5f) + Vector3.up * 0.25f;

        _lines.Add(go);

    }

    void SetPoint(Vector3 point)
    {

        if (_measurements.Count == 0 || _measurements.Last().IsInit)
        {
            _measurements.Add(new Measurement());
            _measurements.Last().P1 = point;
            return;
        }
        _measurements.Last().P2 = point;
        _measurements.Last().IsInit = true;
        CreateMeasuringLine(_measurements.Last());
    }


}

[Serializable]
class Measurement
{
    public Vector3 P1, P2;
    public bool IsInit = false;
}
