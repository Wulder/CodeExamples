using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTester : MonoBehaviour
{

    [SerializeField] private KeyCode _key;
    void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, LayerMask.GetMask("Wall")))
            {
                Debug.Log("Cast!");
            }
        }
    }
}
