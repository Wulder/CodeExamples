using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    [SerializeField] private WallsPlan _plan;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            CreateFloor();
        }
    }
    void CreateFloor()
    {
        Debug.Log("Creating floor...");
    }
}
