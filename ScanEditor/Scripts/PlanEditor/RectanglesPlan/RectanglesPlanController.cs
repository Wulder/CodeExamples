using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RectanglesPlanController : MonoBehaviour
{

    public event Action<RectanglePoint> OnStartCreateRectangle;

    [SerializeField] private RectanglesPlan _plan;
    [SerializeField] private RectanglePoint _pointPrefab;
    [SerializeField] private bool _showGhostPoint = true;
    [SerializeField] private LayerMask _pointLayer;

    private RectanglePoint _selectedPoint;

    private GameObject _ghostPoint;

    private RectanglePoint _startPoint = null, _endPoint = null;

    private List<ControlRectangle> _controls = new List<ControlRectangle>();
    private Dictionary<RectanglePoint, ControlRectangle> _hash = new Dictionary<RectanglePoint, ControlRectangle>();

    private PlanTool<RectanglesPlan> _planTool;
    private void Awake()
    {
        var gm = Resources.Load("Tools/WallsEditor/GhostPoint", typeof(GameObject)) as GameObject;
        _ghostPoint = GameObject.Instantiate(gm);
    }
 

    public void SetNewTool(PlanTool<RectanglesPlan> newTool)
    {
        _planTool?.Disable();
        _planTool = newTool;
        _planTool?.Enable();
    }

    void Update()
    {
        Inputs();
        _planTool?.ToolInput();
    }

    

    #region Inputs
    void Inputs()
    {
        bool blockedByIMGUI = GUIUtility.hotControl != 0;
        if (!EventSystem.current.IsPointerOverGameObject() && !blockedByIMGUI)
        {
            UpdateGhostPointPos();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetNewTool(null);
        }
    }

    void UpdateGhostPointPos()
    {
        if (_showGhostPoint)
        {
            _ghostPoint.SetActive(true);
        }
        else
        { _ghostPoint.SetActive(false); }

        _ghostPoint.transform.position = _plan.PositionOnPlane;
    }
    
    #endregion

    
    private void OnDrawGizmos()
    {
        if (_startPoint)
        {
            Gizmos.color = Color.yellow;
            DrawGizmosRectangle(_startPoint.Position, _plan.PositionOnPlane);
        }

        Gizmos.color = Color.green;
        foreach (var cr in _controls)
        {
            DrawGizmosRectangle(cr.p1.Position, cr.p2.Position);
        }
    }
    private void DrawGizmosRectangle(Vector3 p1, Vector3 p2)
    {
        Gizmos.DrawLine(p1, new Vector3(p2.x, 0, p1.z));
        Gizmos.DrawLine(p1, new Vector3(p1.x, 0, p2.z));

        Gizmos.DrawLine(new Vector3(p2.x, 0, p1.z), p2);
        Gizmos.DrawLine(new Vector3(p1.x, 0, p2.z), p2);
    }
}

struct ControlRectangle
{
    public ControlRectangle(RectanglePoint point1, RectanglePoint point2, Rectangle rect)
    {
        p1 = point1;
        p2 = point2;
        Rectangle = rect;
    }

    public RectanglePoint p1, p2;
    public Rectangle Rectangle;
}
