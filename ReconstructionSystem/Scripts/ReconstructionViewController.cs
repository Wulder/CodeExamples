using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconstructionViewController : MonoBehaviour
{
    private ReconstructionInfo _recInfo;
    private List<Vector3> _occupiedHashes = new List<Vector3>();
    [SerializeField] private Camera_Controller _cc;
    [SerializeField] private ReconstructionLoader _loader;
    private void Awake()
    {
        ReconstructionInfo.OnCreated += SetReconstructionInfo;
        
    }

    private void OnDisable()
    {
        _recInfo.PointBuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _cc.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _cc.enabled = true;
        }
    }

    public void ResetCameraPosition()
    {
        Camera.main.transform.position = _recInfo.Pivot;
    }

    void SetReconstructionInfo(ReconstructionInfo info)
    {
        _recInfo = info;
    }

   

    
}
