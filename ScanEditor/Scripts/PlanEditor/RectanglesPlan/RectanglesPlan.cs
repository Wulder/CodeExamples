using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RectanglesPlan : Plan
{

    public event Action<Rectangle> OnAddRectangle;
    public event Action<Rectangle> OnDeleteRectangle;
    public event Action<Rectangle> OnUpdateRectangle;

    private Transform _pointsRoot, _rectanglesRoot;

    [SerializeField] private RectanglePlanUI _ui;
    public Transform PointsRoot => _pointsRoot;
    public Transform RectanglesRoot => _rectanglesRoot;

    [SerializeField] private List<Rectangle> _rectangles = new List<Rectangle>();
    public List<Rectangle> Rectangles => _rectangles;

    private IRectangleDeserializer _deserializer;
    private IRectangleSerializer _serializer;

    public override void HideUI()
    {
        _ui.gameObject.SetActive(false);
    }

    public override void ShowUI()
    {
        _ui.gameObject.SetActive(true);
    }
    private void Awake()
    {
        _pointsRoot = new GameObject("PointsRoot").transform;
        _pointsRoot.SetParent(transform);
        _rectanglesRoot = new GameObject("RectanglesRoot").transform;
        _rectanglesRoot.SetParent(transform);

        var saver = new RectangleBinarySaver(@$"savedPlan.plan");
        _serializer = saver;
        _deserializer = saver;
    }

    public Rectangle AddRectangle(Vector3 p1, Vector3 p2)
    {
        Rectangle rect = new Rectangle(p1, p2);
        _rectangles.Add(rect);
        OnAddRectangle(rect);
        return rect;
    }

    void AddRectangle(Rectangle rect)
    {
        _rectangles.Add(rect);
        OnAddRectangle?.Invoke(rect);
    }

    public void RemoveRectangle(Rectangle rectangle)
    {
        _rectangles.Remove(rectangle);
        OnDeleteRectangle?.Invoke(rectangle);
    }

    public void UpdateRectangle(Rectangle rect, Vector3 p1, Vector3 p2)
    {

        rect.p1 = p1; rect.p2 = p2;
        rect.Holes.Clear();
        OnUpdateRectangle?.Invoke(rect);
        
        
    }

    public void LoadPlan(List<Rectangle> rects)
    {
        

        Debug.Log("Start remove rectangles");
        foreach(var r in _rectangles.ToArray())
            RemoveRectangle(r);
        _rectangles.Clear();

        Debug.Log("Start creating rectangles");

       
        foreach(var r in rects)
        {
            AddRectangle(r);
        }
    }

    public override void PlanInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
            _serializer?.Serialize(_rectangles);

        if (Input.GetKeyDown(KeyCode.L))
            LoadPlan(_deserializer?.Deserialize());
    }

    #region Inputs
    //void CreateStartPoint()
    //{
    //    _startRectanglePoint = CreatePoint();
    //    _startRectanglePoint.transform.position = _positionOnPlane;
    //    OnAddStarPoint?.Invoke(_startRectanglePoint);
    //}

    //void CreateEndPoint()
    //{
    //    var endPoint = CreatePoint();
    //    endPoint.transform.position = _positionOnPlane;

    //    AddRectangle(new Rectangle(_startRectanglePoint, endPoint));
    //    _startRectanglePoint = null;
    //}

    //void TranslatePoint()
    //{
    //    if (!_selectedPoint) SelectPoint();

    //    if(_selectedPoint && _selectedPoint.Position != _positionOnPlane)
    //    {
    //        _selectedPoint.transform.position = _positionOnPlane;
    //        _rectanglesHash[_selectedPoint].Holes.Clear();
    //        OnTranslatePoint?.Invoke(_selectedPoint);
    //    }

    //}

    //void DeleteRectangle()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
    //    RaycastHit hit = new RaycastHit();
    //    if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
    //    {
    //        RectangleMesh rm = hit.collider.GetComponent<RectangleMesh>();
    //        if (rm)
    //        {
    //            RemoveRectangle(rm.Rectangle);
    //        }
    //    }


    //}

    //void SelectPoint()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
    //    RaycastHit hit = new RaycastHit();
    //    if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, _pointLayer))
    //    {
    //        _selectedPoint = hit.collider.GetComponent<RectanglePoint>();
    //    }
    //}

    //void ResetSelectedPoint()
    //{
    //    _selectedPoint = null;
    //}

    //void AddHole()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
    //    RaycastHit hit = new RaycastHit();
    //    if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
    //    {
    //        RectangleMesh rm = hit.collider.GetComponent<RectangleMesh>();
    //        if (rm)
    //        {
    //            var hole = new RectangleHole();

    //            hole.Normal = hit.normal.y != 0 ? Vector3.forward : hit.normal;
    //            hole.Size = Vector2.one;
    //            hole.Position = Vector2.zero;

    //            rm.Rectangle.Holes.Add(hole);
    //            OnHoleAdd?.Invoke(hole, rm);
    //        }
    //    }
    //}



    #endregion


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green; 
        //foreach (var rect in _rectangles)
        //{
        //    DrawGizmosRectangle(rect.p1.Position, rect.p2.Position);
        //}

        //if(_startRectanglePoint)
        //{
        //    Gizmos.color = Color.yellow;
        //    DrawGizmosRectangle(_startRectanglePoint.Position, _positionOnPlane);
        //}
    }

    private void DrawGizmosRectangle(Vector3 p1, Vector3 p2)
    {
        Gizmos.DrawLine(p1, new Vector3(p2.x, 0, p1.z));
        Gizmos.DrawLine(p1, new Vector3(p1.x, 0, p2.z));

        Gizmos.DrawLine(new Vector3(p2.x, 0, p1.z), p2);
        Gizmos.DrawLine(new Vector3(p1.x, 0, p2.z), p2);
    }

    public static RectanglePoints GetRectPoints(Vector3 start, Vector3 end)
    {
        var point1 = start;
        var point2 = new Vector3(end.x, 0, start.z);
        var point3 = end;
        var point4 = new Vector3(start.x, 0, end.z);

        return new RectanglePoints(point1, point2, point3, point4);
    }
}

[Serializable]
public class Rectangle
{
    public Rectangle(Vector3 start, Vector3 end)
    {
        p1 = start;
        p2 = end;
        Holes = new List<RectangleHole>();
    }

    public List<RectangleHole> Holes;

    public RectanglePoints GetPoints()
    {
        var point1 = p1;
        var point2 = new Vector3(p2.x, 0, p1.z);
        var point3 = p2;
        var point4 = new Vector3(p1.x, 0, p2.z);

        return new RectanglePoints(point1, point2, point3, point4);
    }

    public Vector3 p1, p2;
}

public struct RectanglePoints
{
    public Vector3 p1, p2, p3, p4;

    public RectanglePoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        p1 = point1;
        p2 = point2;
        p3 = point3;
        p4 = point4;
    }
}

[Serializable]
public class RectangleHole
{
    public Vector3 Normal;
    public Vector2 Size;
    public Vector2 Position;
}
