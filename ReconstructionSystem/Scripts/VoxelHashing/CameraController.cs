using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
     private bool _isFollowing = true;

    [SerializeField] private Transform _target;
    [SerializeField] private Camera_Controller _cameraController;
    
    
    

    public void Switch()
    {
        
        if(!_isFollowing)
        {
            _cameraController.enabled = false;
        }
        else
        {
            _cameraController.enabled = true;
        }

        _isFollowing = !_isFollowing;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isFollowing)
        {
            transform.position = _target.transform.position;
            transform.rotation = _target.transform.rotation;
        }
    }
}
