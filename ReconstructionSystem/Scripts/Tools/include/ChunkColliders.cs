using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChunkColliders : MonoBehaviour
{
    private int _octreeDepth;
    private int _root;
    private int _index;
    private Vector3 _position;
    [SerializeField] private GameObject _colliderPredfab;

    public void Init(int o, int r, int index, Vector3 pos, PointBuffer pBuffer)
    {
        Reinit();
        _octreeDepth = o;
        _root = r;
        _position = pos;
        _index = index;
        float size = 1 / Mathf.Pow(2, _octreeDepth);
        transform.localScale = new Vector3(size, size, size);
        transform.position = pos + new Vector3(size / 2, size / 2, size / 2);

        int chunkSize = (int)Mathf.Pow(_root, 3);
        Point[] points = new Point[chunkSize];
        pBuffer.GetData(points,0,_index,chunkSize);

        for (int i = 0; i < Mathf.Pow(_root, 3); i++)
        {
            if(points[i].Position != Vector3.zero)
            {
                GameObject collider = Instantiate(_colliderPredfab);
                collider.transform.SetParent(transform);
               
                float scale = 1.0f / _root;
                collider.transform.localScale = new Vector3(scale, scale, scale);
                collider.transform.localPosition = ((Vector3)VoxelReconstruction.GetPosByIndex(i, _root, _root) * scale) - Vector3.one / 2;
                collider.gameObject.name = (index+i).ToString();
            }
           
        }
    }

    public void Reinit()
    {
        Transform[] c = transform.GetComponentsInChildren<Transform>();
        foreach (Transform gObject in c)
        {
            if (gObject != transform)
                Destroy(gObject.gameObject);
        }
    }
}
