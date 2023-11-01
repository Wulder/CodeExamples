using ProceduralToolkit.Skeleton;
using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;

public class RectanglesCreatorTool : PlanTool<RectanglesPlan>
{
    private RectanglePoint _pointPrefab;

    private RectanglePoint _startPoint = null, _endPoint = null;

    private LayerMask _pointLayer;

    private LineRenderer _buildingLine;

    private RectanglePoint _translatedPoint;

    private Dictionary<RectanglePoint, Rectangle> _controllers = new Dictionary<RectanglePoint, Rectangle>();
    public RectanglesCreatorTool(RectanglesPlan plan) : base(plan)
    {
        _pointLayer = LayerMask.GetMask("PlanPoint");
    }

    public override void Enable()
    {
        base.Enable();
        GameObject gm = Resources.Load("Tools/RectangleEditor/RectanglePoint", typeof(GameObject)) as GameObject;
        _pointPrefab = gm.GetComponent<RectanglePoint>();

        InitControllers();
    }

    public override void Disable()
    {
        base.Disable();
        if (_startPoint) GameObject.Destroy(_startPoint.gameObject);
        if (_endPoint) GameObject.Destroy(_endPoint.gameObject);
        if(_buildingLine) GameObject.Destroy(_buildingLine.gameObject);
        ClearControllers();
    }
    public override void ToolInput()
    {
        base.ToolInput();
        if (_buildingLine)
        {
            var rect = new Rectangle(_startPoint.Position, _plan.PositionOnPlane);
            RectangleLineDrawer.UpdateLinePositions(_buildingLine, rect);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetMouseButtonUp(0)) { _translatedPoint = null; }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetMouseButton(0))
            {
                TranslatePoint();
            }
            return;
        }

        

        if (Input.GetMouseButtonDown(0))
        {
            AddPoint();
            return;
        }
    }

    void AddPoint()
    {
        if (!_startPoint)
        {
            var p = CreatePoint();
            _startPoint = p;
            var rect = new Rectangle(_startPoint.Position, _startPoint.Position);
            _buildingLine = RectangleLineDrawer.CreateLine(rect);
            return;
        }
        else
        {
            var p = CreatePoint();
            _endPoint = p;

            var rect = _plan.AddRectangle(_startPoint.Position, _endPoint.Position);

            GameObject.Destroy(_startPoint.gameObject);
            GameObject.Destroy(_endPoint.gameObject);
            _endPoint = null;
            _startPoint = null;
            GameObject.Destroy(_buildingLine.gameObject);
            CreateRectangleController(rect);


            return;
        }
    }
    void TranslatePoint()
    {
        if (!_translatedPoint) _translatedPoint = SelectPoint();
        if(_translatedPoint)
        {
           
            var rect = _controllers[_translatedPoint];
           

            if(_translatedPoint.Position == rect.p1)
            {
                if(_plan.PositionOnPlane != rect.p2)
                {
                    _plan.UpdateRectangle(rect, _plan.PositionOnPlane, rect.p2);
                    _translatedPoint.transform.position = _plan.PositionOnPlane;
                }
                
              
            }
            if (_translatedPoint.Position == rect.p2)
            {
                if (_plan.PositionOnPlane != rect.p1)
                {
                    _plan.UpdateRectangle(rect, rect.p1, _plan.PositionOnPlane);
                    _translatedPoint.transform.position = _plan.PositionOnPlane;
                }
            }       
        }
    }
    void CreateRectangleController(Rectangle rect)
    {
        var p1 = GameObject.Instantiate(_pointPrefab);
        var p2 = GameObject.Instantiate(_pointPrefab);

        p1.transform.SetParent(_plan.PointsRoot);
        p2.transform.SetParent(_plan.PointsRoot);

        p1.transform.position = rect.p1;
        p2.transform.position = rect.p2;

        _controllers[p1] = rect;
        _controllers[p2] = rect;
    }

    void ClearControllers()
    {
        var keys = _controllers.Keys;
        foreach (var key in keys)
            GameObject.Destroy(key.gameObject);

        _controllers.Clear();
    }

    void InitControllers()
    {
        foreach(var rect in _plan.Rectangles)
            CreateRectangleController(rect);
    }
    RectanglePoint SelectPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, _pointLayer))
        {
            return hit.collider.GetComponent<RectanglePoint>();
        }
        return null;
    }

    RectanglePoint CreatePoint()
    {
        var p = GameObject.Instantiate(_pointPrefab);
        p.transform.SetParent(_plan.PointsRoot);
        p.transform.position = _plan.PositionOnPlane;
        return p;
    }
}
