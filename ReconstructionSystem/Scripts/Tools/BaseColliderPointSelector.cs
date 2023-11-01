using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class BaseColliderPointSelector : PointSelector
{

    [SerializeField] private float _step;
    [SerializeField] private int _iterations;
    [SerializeField] private GameObject _colliders, _chunksParent;
    [SerializeField] private SubCollider _subColliderPrefab;
    [SerializeField] private ChunkColliders _chunkColliders;
    [SerializeField] private LayerMask _castLayers, _pointsCastLayers;
    [SerializeField] private int _layer;
 
    
    private Ray _ray;
    private List<SubCollider> _subColldiers = new List<SubCollider>();
    private bool _startCast = false;
    private GameObject _selected1, _selected2;

    private Dictionary<int, Vector3Int> _occupiedHashes = new Dictionary<int, Vector3Int>();
    private List<SubCollider> _chunks = new List<SubCollider>();

    private int _currentIteration, _chunkIteration;
    private bool _readyCast = false;
    private bool _readyChunks = false;
    private bool _readyCastChunk = false;
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _currentIteration = 0;
            _startCast = true;
            _readyCast = false;
            
            _occupiedHashes.Clear();
            _occupiedHashes = GetHashes(_ray, _iterations, _step);
        }

        if (_startCast && _currentIteration < _occupiedHashes.Count)
        {
            if (_readyCast)
            {
                RaycastHit hit;
                if (CastRay(_ray, out hit, _castLayers))
                {


                    SubCollider sc = hit.collider.gameObject.GetComponent<SubCollider>();
                    if (!_chunks.Contains(sc))
                    {
                        sc.gameObject.layer = 0;
                        sc.transform.SetParent(_chunksParent.transform);
                        _subColldiers.Remove(sc);
                        _chunks.Add(sc);
                        _chunkIteration = _chunks.Count - 1;
                        _readyChunks = true;
                    }
                    
                    // _chunkColliders.Init(_recInfo.OctreeDepth,_recInfo.ChunkDensity,sc.LvlIndex,sc.Pos,_recInfo.PointBuffer);
                    // Physics.SyncTransforms();
                    // DestroySubColliders();
                    //_startCast = false;
                    //_readyCastChunk = true;
                }
                else
                {
                    _currentIteration++;
                    _readyCast = false;
                }
            }
            else
            {
                CreateOctreeByHash(_occupiedHashes.ElementAt(_currentIteration).Key);
                Physics.SyncTransforms();
                _readyCast = true;
            }
            //return;
        }
        else if(!_readyChunks && _chunks.Count > 0)
        {
            _startCast = false;
            
            Debug.Log("rady chanks");
        }


        if(_readyCastChunk)
        {
            RaycastHit hit;
            if (Physics.Raycast(_ray, out hit, 1000, _pointsCastLayers))
            {

                InvokeNewPoint(hit);
                ClearChunks();
                _readyChunks = false;
                _startCast = false;
                DestroySubColliders();
            }
            _readyCastChunk = false;
        }

        if (_readyChunks && _chunkIteration < _chunks.Count)
        {
            _chunkColliders.Reinit();
            _chunkColliders.Init(_recInfo.OctreeDepth, _recInfo.ChunkDensity, _chunks[_chunkIteration].LvlIndex, _chunks[_chunkIteration].Pos, _recInfo.PointBuffer);
            Physics.SyncTransforms();
            _readyCastChunk = true;

            _chunkIteration++;
        }
        else
        {
            ClearChunks();
            _readyChunks = false;
        }

    }

    private void Start()
    {
        Physics.autoSyncTransforms = true;
    }

    
    void ClearChunks()
    {
        foreach(SubCollider sc in _chunks)
        {
            Destroy(sc.gameObject);
        }
        _chunks.Clear();
            
    }

    bool CastRay(Ray ray, out RaycastHit outHit, LayerMask mask)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            if (hit.collider.gameObject.GetComponent<SubCollider>().Lvl == _recInfo.OctreeDepth - 1)
            {
                outHit = hit;
                return true;
            }
        }
        outHit = hit;
        return false;
    }

    List<SubCollider> GetChunks(Ray ray)
    {
        List<SubCollider> chunks = new List<SubCollider>();

        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, _castLayers);

        foreach (RaycastHit h in hits)
            chunks.Add(h.collider.gameObject.GetComponent<SubCollider>());

        return chunks;
    }

    void CreateOctreeByHash(int hash)
    {
        DestroySubColliders();

        _colliders.transform.position = _occupiedHashes[hash] + Vector3.one / 2;
        CreateOctree(0, _recInfo._hashMap[hash], Vector3.zero);
    }
    void CreateOctree(int lvl, int index, Vector3 startPos)
    {
        for (int k = 0; k < 8; k++)
        {
            if (_recInfo._lvls[lvl][index + k] != -1)
            {
                SubCollider c = Instantiate(_subColliderPrefab, _colliders.transform);
                _subColldiers.Add(c);
                Vector3 pos = startPos + VoxelReconstruction.GetDevidedVecByIndex(k, lvl);
                c.transform.localPosition = (-Vector3.one / 2) + pos;
                c.transform.localScale = new Vector3(VoxelReconstruction._vecValues[lvl], VoxelReconstruction._vecValues[lvl], VoxelReconstruction._vecValues[lvl]);
                c.Init(lvl, _recInfo._lvls[lvl][index + k], c.transform.position);

                if (lvl < _recInfo.OctreeDepth - 1)
                {
                    CreateOctree(lvl + 1, _recInfo._lvls[lvl][index + k], pos);
                }
                else
                {
                    c.gameObject.layer = _layer;
                }

            }
        }
    }

    void DestroySubColliders()
    {
        foreach (SubCollider c in _subColldiers)
        {
            Destroy(c.gameObject);
        }
        _subColldiers.Clear();

    }
   
    
    Dictionary<int, Vector3Int> GetHashes(Ray ray, int iterations, float step)
    {
        Dictionary<int, Vector3Int> hashes = new Dictionary<int, Vector3Int>();

        for (int i = 0; i < iterations; i++)
        {
            Vector3 pos = ray.origin + ray.direction * (_step * i);
            Vector3Int flooredPos = Vector3Int.FloorToInt(pos);
            int hash = VoxelReconstruction.GetIndexByPos(flooredPos, _recInfo.RootSize);
            hash = Mathf.Clamp(hash, 0, (int)Mathf.Pow(_recInfo.RootSize, 3));
            if (!hashes.ContainsKey(hash) && _recInfo._hashMap[hash] != -1)
            {
                hashes[hash] = flooredPos;
            }
        }

        return hashes;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * 1000, Color.green);



    }

    struct CastSphere
    {
        public CastSphere(Vector3 p, float s)
        {
            pos = p;
            size = s;
        }

        public Vector3 pos;
        public float size;
    }
}





