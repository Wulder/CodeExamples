using System;
using UnityEngine;

public class MeshRoot : MonoBehaviour
{
    public event Action<GameObject> OnNewMesh;
    [SerializeField] private bool _drawBounds;

     private GameObject _child;
     private Mesh _mesh;
    void Start()
    {
        _child = transform.GetChild(0).gameObject;
        _mesh = _child.GetComponent<MeshFilter>().mesh;
    }

  

    
    public static void AlignHeight(GameObject mesh)
    {
        
        float yOffset = mesh.transform.position.y + mesh.GetComponent<MeshFilter>().mesh.bounds.min.y;
        yOffset = yOffset;
        if(yOffset > 0)
        {
            mesh.transform.position -= new Vector3(0, Mathf.Abs(yOffset), 0);
        }
        else
        {
            mesh.transform.position += new Vector3(0, Mathf.Abs(yOffset), 0);
        }
        
    }
    private void OnDrawGizmos()
    {
        if(_drawBounds && _mesh)
        {
            Color c = Color.green; c.a = 0.5f;

            Gizmos.color = c;
            Bounds bounds = _mesh.bounds;
            Gizmos.DrawCube(bounds.center + _child.transform.position, bounds.size);
        }
    }

    public void SetNewMesh(GameObject mesh)
    {
        Destroy(_child);
        MeshRenderer mr = mesh.GetComponentInChildren<MeshRenderer>();
   
        mr.gameObject.transform.SetParent(transform);
        _child = mr.gameObject;
        _mesh = _child.GetComponent<MeshFilter>().mesh;
        AlignHeight(mesh);

        mr.gameObject.AddComponent<MeshCollider>();
        mr.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        OnNewMesh?.Invoke(mr.gameObject);
    }

    public void ConfigureMaterial()
    {
        MeshRenderer mr = _child.GetComponent<MeshRenderer>();
        Shader shader = Shader.Find("Shader Graphs/TwoDirectionSlice");
        Texture tex = mr.material.mainTexture;

        mr.material.shader = shader;
        mr.material.SetTexture("_Texture", tex);
        mr.material.SetFloat("_UpThreshold", 10);
        mr.material.SetFloat("_DownThreshold", -10);
        
        
    }
}
