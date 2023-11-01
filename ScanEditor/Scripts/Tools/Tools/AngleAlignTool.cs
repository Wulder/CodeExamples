using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AngleAlignTool : Tool
{
    

    private Vector3 _corner;
    private List<RaycastHit> _points = new List<RaycastHit>();
    private GameObject _cornerObject;

    private GameObject _pointPrefab;  

    private AngleAlignUI _ui;

    private List<GameObject> _gizmosPoints = new List<GameObject>();
    public AngleAlignTool()
    {
        _points.Clear();
    }

    public override void Enable()
    {
        GameObject gm = Resources.Load("ToolsUI/AngleAlignUI", typeof(GameObject)) as GameObject;
        _pointPrefab = Resources.Load("Tools/CornerPoint", typeof(GameObject)) as GameObject;

        _ui = GameObject.Instantiate(gm).GetComponent<AngleAlignUI>();
        _ui.Init(this);
    }

    public override void Disable()
    {
        Clear(); 
        GameObject.Destroy(_ui.gameObject);
    }
    public override void ToolInput()
    {
        bool blockedByIMGUI = GUIUtility.hotControl != 0;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !blockedByIMGUI)
        {
            if (_points.Count >= 2)
            {
                Clear();
            }
                

            SelectPoint();

            if (_points.Count == 2)
            {
                CreateCornerPoint();
            }
                
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
        
        foreach (var point in _gizmosPoints) GameObject.Destroy(point);
        _gizmosPoints.Clear();

        _corner = Vector3.zero;
        GameObject.Destroy(_cornerObject);
    }
    void SelectPoint()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100) && hit.collider.gameObject == ApplicationController.Instance.MainMesh)
        {
            _points.Add(hit);
            
            var gizmosPoint = GameObject.Instantiate(_pointPrefab);
            gizmosPoint.transform.position = hit.point;
            gizmosPoint.transform.up = hit.normal;

            _gizmosPoints.Add(gizmosPoint);
        }
    }

    public void Align()
    {

        if (_cornerObject == null)
            return;

        var q = Quaternion.FromToRotation(_cornerObject.transform.forward, Vector3.forward);
        ApplicationController.Instance.MainMesh.transform.rotation *= q;
        Clear();
        
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
    public override void DrawGizmos()
    {
        //foreach (var point in _points)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(point.point, point.point + point.normal);
        //    Gizmos.DrawSphere(point.point, 0.05f);
        //}

        //Gizmos.color = Color.red;
        //if (_points.Count == 2)
        //    Gizmos.DrawSphere(_corner, 0.05f);
    }
    public override void DrawGUI()
    {
        
        //Rect rect = new Rect(50, 50, 500, 500);
        //GUI.Label(rect, $"INFO:");
        //if (_points.Count == 2)
        //{
        //    rect.y += 50;
        //    GUI.Label(rect, $"P1: {_points[0].point}");
        //    rect.y += 50;
        //    GUI.Label(rect, $"P2: {_points[1].point}");
        //    rect.y += 50;
        //    GUI.Label(rect, $"Angle: {Vector3.Angle(_points[0].normal, _points[1].normal)}");
        //    rect.y += 50;

        //    if (GUI.Button(new Rect(rect.x, rect.y, 50, 25), "Align"))
        //    {
        //        Align();
        //    }
        //}
    }
}
