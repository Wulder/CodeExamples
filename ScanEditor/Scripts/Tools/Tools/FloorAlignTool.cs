using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorAlignTool : Tool
{
  
  

    private Material _planeMaterial;
    private GameObject _mesh;
    private GameObject _plane;
    
    private FloorAlignUI _ui;

    public List<RaycastHit> _planePoints { get; private set; } = new List<RaycastHit>();
    

    public FloorAlignTool(GameObject mesh)
    {
        _planeMaterial = ApplicationController.Instance.AppConfig.ProjectMaterial;
        _mesh = mesh;
    }
    public override void ToolInput()
    {
        bool blockedByIMGUI = GUIUtility.hotControl != 0;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !blockedByIMGUI)
        {
            if (_planePoints.Count >= 3)
                ClearPlane();

            SelectPoint();

            if (_planePoints.Count == 3)
                CreatePlane();
        }
    }

    public override void Enable()
    {
        GameObject gm = Resources.Load("ToolsUI/FloorAlignUI", typeof(GameObject)) as GameObject;
        _ui = GameObject.Instantiate(gm).GetComponent<FloorAlignUI>();
        _ui.Init(this);
    }
    public override void Disable()
    {
        GameObject.Destroy(_ui.gameObject);
        ClearPlane();
    }
    void SelectPoint()
    {
        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            _planePoints.Add(hit);
        }

     

    }

    [ContextMenu("CreatePlane")]
    void CreatePlane()
    {
        _plane = new GameObject("Plane");
        _plane.transform.rotation = new Quaternion(0, 0, 0, 0);
        Vector3 midPoint = (_planePoints[0].point + _planePoints[1].point + _planePoints[2].point) / 3;
        _plane.transform.position = midPoint;

        MeshRenderer mr = _plane.AddComponent<MeshRenderer>();
        Mesh mesh = _plane.AddComponent<MeshFilter>().mesh;


        //change components
        mr.sharedMaterial = _planeMaterial;
        mesh.vertices = _planePoints.Select(hit => hit.point - midPoint).ToArray();
        mesh.triangles = new int[] { 0, 1, 2 };


    }

    [ContextMenu("Clear")]
    public void ClearPlane()
    {
     
        _planePoints.Clear();
         GameObject.Destroy(_plane);
    }

    [ContextMenu("AlignPlane")]
    private void AlignPlane()
    {
        Vector3 norm = (Vector3.Cross(_planePoints[1].point - _planePoints[0].point, _planePoints[2].point - _planePoints[0].point)).normalized;
        norm = norm.y < 0 ? norm * -1 : norm;
        
        _plane.transform.rotation = Quaternion.FromToRotation(norm, Vector3.up);


    }

    [ContextMenu("AlignMesh")]
    public void AlignMesh()
    {
        if (_planePoints.Count < 3)
            return;

            AlignPlane();
        _mesh.transform.rotation *= _plane.transform.rotation;
        _mesh.transform.position = new Vector3(_mesh.transform.position.x, 0, _mesh.transform.position.z);
        ClearPlane();
        MeshRoot.AlignHeight(_mesh);
        
    }

    public override void DrawGizmos()
    {
        
        for (int i = 0; i < _planePoints.Count; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_planePoints.Select(hit => hit.point).ToArray()[i], 0.05f);

            Gizmos.color = Color.green;
            if (i < _planePoints.Count - 1)
            {
                Gizmos.DrawLine(_planePoints.Select(hit => hit.point).ToArray()[i], _planePoints.Select(hit => hit.point).ToArray()[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(_planePoints.Select(hit => hit.point).ToArray()[i], _planePoints.Select(hit => hit.point).ToArray()[0]);
            }

        }

        if (_planePoints.Count == 3)
        {
            Vector3 pNorm = (Vector3.Cross(_planePoints[1].point - _planePoints[0].point, _planePoints[2].point - _planePoints[0].point)).normalized;
            Debug.DrawLine(_plane.transform.position, _plane.transform.position + pNorm);
        }

    }

    public override void DrawGUI()
    {
        //Rect rect = new Rect(50, 50, 200, 100);
        //if (UnityEngine.GUI.Button(rect, "AlignMesh"))
        //{
        //    AlignMesh();
        //}
    }
}
