using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsOutline : MonoBehaviour
{
    [SerializeField] private WallsPlan _plan;
    [SerializeField] private LineRenderer _linePrefab;

    private Transform _linesParent;

    private Dictionary<Wall,LineRenderer> _lines = new Dictionary<Wall,LineRenderer>();

    private void OnEnable()
    {
        _plan.OnAddWall += CreateLine;
        _plan.OnRemoveWall += DeleteLine;
        _plan.OnPointTranslate += OnTranslatePoint;
    }

    private void OnDisable()
    {
        _plan.OnAddWall -= CreateLine;
        _plan.OnRemoveWall -= DeleteLine;
        _plan.OnPointTranslate -= OnTranslatePoint;
    }
    void Start()
    {
        GameObject gm = new GameObject("Lines Parent");
        gm.transform.SetParent(transform, false);
        _linesParent = gm.transform;
    }

    void CreateLine(Wall wall)
    {
        var line = Instantiate(_linePrefab);
        line.transform.SetParent(_linesParent, false);
        line.transform.position = Vector3.zero;

        line.SetPosition(0, wall.Point1.Position);
        line.SetPosition(1, wall.Point2.Position);

        _lines[wall] = line;
    }

    void DeleteLine(Wall wall)
    {
        Destroy(_lines[wall]);
    }

    void UpdateLine(Wall wall)
    {
        var line = _lines[wall];

        line.SetPosition(0, wall.Point1.Position);
        line.SetPosition(1, wall.Point2.Position);
    }

    void OnTranslatePoint(WallsEditorPoint point)
    {
        foreach(var wall in point._includedWalls)
        {
            UpdateLine(wall);
        }
    }
}
