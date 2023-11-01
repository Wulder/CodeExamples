using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DecreaseDensityByDistance : MonoBehaviour
{
    [SerializeField] private List<VisualEffect> _clouds = new List<VisualEffect>();

    [SerializeField] private float _maxPoints, _distanceScale;
    void Update()
    {
        UpdateClouds();
        if(Input.GetKeyDown(KeyCode.E))
        {
            UpdateClouds();
        }
    }

    void UpdateClouds()
    {
        foreach(VisualEffect effect in _clouds)
        {
            float pCount = _maxPoints - (Vector3.Distance(effect.transform.position,transform.position)*_distanceScale);
            effect.SetFloat(Shader.PropertyToID("pCount"), pCount);
            effect.Reinit();
        }
    }
}
