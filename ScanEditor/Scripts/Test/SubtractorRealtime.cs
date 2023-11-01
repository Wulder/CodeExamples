using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class SubtractorRealtime : MonoBehaviour
{
    [SerializeField] private GameObject _base, _subtractor;
    [SerializeField] private bool _run;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_run) Subtract();
    }

    void Subtract()
    {
        Model result = CSG.Subtract(_base.gameObject, _subtractor);
    }
}
