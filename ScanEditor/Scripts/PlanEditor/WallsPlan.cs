using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsPlan : Plan
{

    public Action<Wall> OnAddWall;
    public Action<Wall> OnRemoveWall;
    public Action<WallsEditorPoint> OnPointTranslate;

    [SerializeField] private List<WallsEditorPoint> _points = new List<WallsEditorPoint>();
    [SerializeField] private WallsEditorPoint _pointPrefab;
    [SerializeField] private List<Wall> _walls = new List<Wall>();
    [SerializeField] private WallsEditorPoint _lastPoint;
    [SerializeField] private LayerMask _pointLayer;


    public List<Wall> Walls => _walls;
    public void AddPoint(Vector2 point)
    {
        Vector3 pos = point.GetPlaneVector();
        var existingPoint = _points.Find(p => p.Position == pos);

        WallsEditorPoint newPoint;
        if(existingPoint == null)
        {
            newPoint = Instantiate(_pointPrefab);
            newPoint.Init(this);
            newPoint.transform.position = pos;
            _points.Add(newPoint);
            newPoint.transform.SetParent(transform);
        }
        else
        {
            newPoint = existingPoint;
        }
        
        if(_lastPoint)
        {
            if (_lastPoint.Position != newPoint.Position)
            {
                Wall w = AddWall(_lastPoint, newPoint);
            }
            _lastPoint.SetColor(Color.red);
        }
        

        
        _lastPoint = newPoint; ;
        _lastPoint.SetColor(Color.green);
    }
    Wall AddWall(WallsEditorPoint p1, WallsEditorPoint p2)
    {
        Wall wall = new Wall();
        wall.Point1 = p1;
        wall.Point2 = p2;
        p1.IncludeWall(wall);
        p2.IncludeWall(wall);
        _walls.Add(wall);
        OnAddWall?.Invoke(wall);
        return wall;
    }
     void RemoveWall(Wall wall)
    {
        _walls.Remove(wall);
        OnRemoveWall?.Invoke(wall);
    }
     void RemovePoint(WallsEditorPoint point)
    {
        foreach (var wall in point._includedWalls)
        {
            var p = wall.Point1 == point ? wall.Point2 : wall.Point1;
            p.RemoveIncludeWall(wall);
            RemoveWall(wall);
        }


        _points.Remove(point);
        Destroy(point.gameObject);
    }
     void SelectPoint(WallsEditorPoint point)
    {
        _lastPoint?.SetColor(Color.red);
        _lastPoint = point;
        _lastPoint?.SetColor(Color.green);
    }
     void ResetSelectedPoint()
    {
        _lastPoint.SetColor(Color.red);
        _lastPoint = null;
    }

    public void TranslatePoint(WallsEditorPoint point, Vector3 newPosition)
    {
        if(point.transform.position != newPosition)
        {
            point.transform.position = newPosition;
            OnPointTranslate?.Invoke(point);
        }  
    }
    private void OnDrawGizmos()
    {

        //foreach (var wall in _walls)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawLine(wall.Point1.Position,wall.Point2.Position);
        //}
    }

    void TranslatePointInput()
    {
        if (!Input.GetMouseButton(0)) return;

        if (_lastPoint == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, _pointLayer))
            {
                _lastPoint = hit.collider.gameObject.GetComponent<WallsEditorPoint>();
            }
        }
        else
        {
            TranslatePoint(_lastPoint, _positionOnPlane);
        }



    }

    void CreatePointInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddPoint(_positionOnPlane.GetPlaneVector());
        }
    }

    void DeletePointInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_lastPoint == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, _pointLayer))
                {
                    _lastPoint = hit.collider.gameObject.GetComponent<WallsEditorPoint>();
                }
            }

            if (_lastPoint != null)
            {
                RemovePoint(_lastPoint);
                _lastPoint = null;
            }
        }

    }

    void SelectPointInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000,_pointLayer))
            {
                var point = hit.collider.gameObject.GetComponent<WallsEditorPoint>();
                if (_lastPoint && point == _lastPoint)
                {
                    ResetSelectedPoint();
                }
                else
                {
                    _lastPoint = point;
                    SelectPoint(_lastPoint);
                }

            }
        }
    }

    public override void PlanInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            TranslatePointInput();
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            DeletePointInput();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectPointInput();
        }
        else
            CreatePointInput();


        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ResetSelectedPoint();
        }
    }
}

[Serializable]
public struct Wall
{
    public WallsEditorPoint Point1, Point2;
}

public static class Vector2Extension
{
    public static Vector3 GetPlaneVector(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
}

public static class Vector3Extension
{
    public static Vector2 GetPlaneVector(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
