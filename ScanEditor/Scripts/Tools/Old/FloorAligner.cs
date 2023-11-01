using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class FloorAligner : OldTool
{
    public event Action OnCofirmed;

    [SerializeField] private Transform _worldRoot;
    [SerializeField] private MeshRoot _meshRoot;

    [SerializeField] private Material _planeMaterial;
    private GameObject _plane;

    [SerializeField] private GameObject _uiConfirmation, _uiAlignButton;
    private List<RaycastHit> _planePoints = new List<RaycastHit>();

    private Vector3 _norm;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (_planePoints.Count >= 3)
                ClearPlane();

            SelectPoint();

            if (_planePoints.Count == 3)
                CreatePlane();
        }
    }
    public override void Disable()
    {
        base.Disable();
        ClearPlane();
    }
    void SelectPoint()
    {
        _uiConfirmation.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            _planePoints.Add(hit);
        }

        if (_planePoints.Count > 2)
        {
            _uiAlignButton.SetActive(true);
        }
        else
            _uiAlignButton.SetActive(false);

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
        _uiConfirmation.SetActive(false);
        _uiAlignButton.SetActive(false);
        _planePoints.Clear();
        Destroy(_plane);
    }

    [ContextMenu("AlignPlane")]
    private void AlignPlane()
    {
        Vector3 norm = (Vector3.Cross(_planePoints[1].point - _planePoints[0].point, _planePoints[2].point - _planePoints[0].point)).normalized;
        norm = norm.y < 0 ? norm * -1 : norm;
        _norm = norm;
        _plane.transform.rotation = Quaternion.FromToRotation(norm, Vector3.up);

       
    }

    [ContextMenu("AlignMesh")]
    public void AlignMesh()
    {
        AlignPlane();
        MeshSelector.SelectedMesh.transform.rotation *= _plane.transform.rotation;
        MeshSelector.SelectedMesh.transform.position = new Vector3(MeshSelector.SelectedMesh.transform.position.x, 0, MeshSelector.SelectedMesh.transform.position.z);
        ClearPlane();
        //_meshRoot.AlignHeight();
        _uiConfirmation.SetActive(true);
    }

    private void OnDrawGizmos()
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

    public void Confirm()
    {
        OnCofirmed?.Invoke();
        Disable();
    }


}
