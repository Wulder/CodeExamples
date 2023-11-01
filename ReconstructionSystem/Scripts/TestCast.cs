using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
     
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log(Physics.Raycast(_ray.origin, _ray.direction, 1000));
        }
    }
}
