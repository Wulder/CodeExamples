using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class RectangleLineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRendererPrefab;
    [SerializeField] private RectanglesPlan _plan;
    [SerializeField] private RectanglesPlanController _planController;

    private Dictionary<Rectangle, LineRenderer> _lines = new Dictionary<Rectangle, LineRenderer>();

    private LineRenderer _buildingLine;
    private Rectangle _buildingRectangle;

    private void OnEnable()
    {
        _plan.OnAddRectangle += AddLine;
        _plan.OnUpdateRectangle += RebuildLine;
        _plan.OnDeleteRectangle += DeleteLine;

        _planController.OnStartCreateRectangle += CreateBuildingLine;
        _plan.OnAddRectangle += DeleteBuildingLine;
    }

    private void OnDisable()
    {
        _plan.OnAddRectangle -= AddLine;
        _plan.OnUpdateRectangle -= RebuildLine;
        _plan.OnDeleteRectangle -= DeleteLine;

        _planController.OnStartCreateRectangle -= CreateBuildingLine;
        _plan.OnAddRectangle -= DeleteBuildingLine;
    }

    private void Update()
    {
        if(_buildingLine != null )
        {
            _buildingRectangle.p2 = _plan.PositionOnPlane;
            UpdateLinePositions(_buildingLine, _buildingRectangle);
        }
    }


    void AddLine(Rectangle rect)
    {
        var line = CreateLine(rect);
        _lines[rect] = line;
    }

    void DeleteLine(Rectangle rect)
    {
        Destroy(_lines[rect].gameObject);
        _lines.Remove(rect);
    }

    void RebuildLine(Rectangle rect)
    {

        var line = _lines[rect];

        UpdateLinePositions(line, rect);


    }

    public static void UpdateLinePositions(LineRenderer line, Rectangle rect)
    {
        var points = rect.GetPoints();

        line.SetPosition(0, points.p1);
        line.SetPosition(1, points.p2);
        line.SetPosition(2, points.p3);
        line.SetPosition(3, points.p4);
    }
    public static LineRenderer CreateLine(Rectangle rect)
    {

        var linePrefab = Resources.Load("Tools/RectangleEditor/Line", typeof(GameObject)) as GameObject;

        var line = Instantiate(linePrefab).GetComponent<LineRenderer>();

        var points = rect.GetPoints();

        line.SetPosition(0, points.p1);
        line.SetPosition(1, points.p2);
        line.SetPosition(2, points.p3);
        line.SetPosition(3, points.p4);

        return line;

    }

    void CreateBuildingLine(RectanglePoint point)
    {
        _buildingLine = Instantiate(_lineRendererPrefab);
        _buildingLine.transform.SetParent(transform);
        _buildingRectangle = new Rectangle(point.Position, _plan.PositionOnPlane);
    }

    void DeleteBuildingLine(Rectangle rect)
    {
        if (!_buildingLine) return;
        Destroy( _buildingLine.gameObject);
        _buildingLine = null;
        _buildingRectangle = null;
    }


}
