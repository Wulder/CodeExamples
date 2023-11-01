using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class NewPointSelector : PointSelector
{

    [SerializeField] private int _iterations;
    [SerializeField] private float _length;
    [SerializeField] private float _step;

    #region Debug
    [Header("DEBUG")]
    [SerializeField] private Vector3 _direction;
    #endregion

    private Block[] _blocks;
    private Ray _ray;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cast();


        }
    }

    void Cast()
    {
        Debug.Log("startCast");
        _blocks = new Block[10];
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 start = _ray.origin;
        Vector3 end = _ray.direction * _length;

        
    }


    private void OnGUI()
    {
        DebugInfo();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Vector3.zero, _direction);
        DrawCubicRay();
        Gizmos.color = Color.green;
        DrawRay();
    }

    private void DebugInfo()
    {
        Rect pos = new Rect(Screen.width/4, Screen.height /2,500,500);
        GUI.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        GUI.Label(pos, $"SIN X: {Mathf.Sin(Vector3.Angle(Vector3.right,_direction)*Mathf.PI/180)}",style);
    }

    private void DrawCubicRay()
    {
        if (_blocks != null)
            foreach (Block b in _blocks)
            {
                Gizmos.color = new Color(1, 1, 0, .2f);
                DrawCube(b.Pos, 1);
                

            }
    }

    private void DrawRay()
    {
        Gizmos.DrawLine(_ray.origin, _ray.direction * 10000);
    }

    private void DrawCube(Vector3 pos, float size)
    {
        Vector3 sizeV = new Vector3(size, size, size);
        Gizmos.DrawCube(pos + sizeV / 2, sizeV);
    }

    struct Block
    {
        public Vector3Int Pos;
        public Vector3 RawPos;

    }
}


