using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    private protected ReconstructionInfo _recInfo;
    void Awake()
    {
        ReconstructionInfo.OnCreated += SetReconstructionInfo;
    }

    void SetReconstructionInfo(ReconstructionInfo info)
    {
        _recInfo = info;

    }
    
}
