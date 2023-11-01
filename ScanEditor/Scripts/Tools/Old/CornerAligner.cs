using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CornerAligner : OldTool
{

    public event Action OnConfirmed;

    private Vector3 _corner;
    private List<RaycastHit> _points = new List<RaycastHit>();
    private GameObject _cornerObject;

    [SerializeField] private GameObject _uiAlignButton, _uiConfirmation;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (_points.Count >= 2)
                Clear();

            SelectPoint();

            if (_points.Count == 2)
                CreateCornerPoint();
        }


    }

    void CreateCornerPoint()
    {
        Vector3 line1Start = new Vector3(_points[0].point.x, _points[0].point.y, _points[0].point.z);
        Vector3 line1End = new Vector3(line1Start.x - _points[1].normal.x * 1000, line1Start.y, line1Start.z - _points[1].normal.z * 1000);

        Vector3 line2Start = new Vector3(_points[1].point.x, _points[0].point.y, _points[1].point.z);
        Vector3 line2End = new Vector3(line2Start.x - _points[0].normal.x * 1000, line1Start.y, line2Start.z - _points[0].normal.z * 1000);

        _corner = GetIntersectionPoint(line1Start, line1End, line2Start, line2End);

        _cornerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _cornerObject.transform.position = _corner;
        _cornerObject.transform.localScale = Vector3.one * 0.1f;
        _cornerObject.transform.forward = _points[0].point - _corner;



    }
    void Clear()
    {
        _points.Clear();
        _corner = Vector3.zero;
        Destroy(_cornerObject);
    }
    void SelectPoint()
    {
        _uiConfirmation.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100) && hit.collider.gameObject == MeshSelector.SelectedMesh)
        {

            _points.Add(hit);
        }

        if (_points.Count > 1)
            _uiAlignButton.SetActive(true);
        else
            _uiAlignButton.SetActive(false);

    }

    [ContextMenu("Align")]
    public void Align()
    {


        var q = Quaternion.FromToRotation(_cornerObject.transform.forward, Vector3.forward);
        MeshSelector.SelectedMesh.transform.rotation *= q;
        Clear();
        _uiConfirmation.SetActive(true);
        _uiAlignButton.SetActive(false);
    }

    public void Confirm()
    {
        OnConfirmed?.Invoke();
        Disable();
    }
    private Vector3 GetIntersectionPoint(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
    {
        // Получаем направления отрезков
        Vector3 line1Dir = line1End - line1Start;
        Vector3 line2Dir = line2End - line2Start;

        // Вычисляем параметры t и u для пересечения отрезков
        float t = Vector3.Cross(line2Start - line1Start, line2Dir).magnitude / Vector3.Cross(line1Dir, line2Dir).magnitude;
        float u = Vector3.Cross(line2Start - line1Start, line1Dir).magnitude / Vector3.Cross(line1Dir, line2Dir).magnitude;

        // Проверяем, находится ли точка пересечения внутри обоих отрезков
        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            // Вычисляем точку пересечения
            Vector3 intersectionPoint = line1Start + line1Dir * t;
            return intersectionPoint;
        }

        return Vector3.zero; // Если отрезки не пересекаются
    }

    private void OnDrawGizmos()
    {

        foreach (var point in _points)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(point.point, point.point + point.normal);
            Gizmos.DrawSphere(point.point, 0.05f);
        }

        Gizmos.color = Color.red;
        if (_points.Count == 2)
            Gizmos.DrawSphere(_corner, 0.05f);


    }

    private void OnGUI()
    {
        return;
        Rect rect = new Rect(50, 50, 500, 500);


        if (_points.Count == 2)
        {
            GUI.Label(rect, $"P1: {_points[0].point}");
            rect.y += 50;
            GUI.Label(rect, $"P2: {_points[1].point}");
            rect.y += 50;
            GUI.Label(rect, $"Angle: {Vector3.Angle(_points[0].normal, _points[1].normal)}");
            rect.y += 50;

            if (GUI.Button(new Rect(rect.x, rect.y, 50, 25), "Align"))
            {
                Align();
            }
        }



    }
}
