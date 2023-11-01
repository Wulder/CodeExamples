using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTReaderTest : MonoBehaviour
{
    [SerializeField] private FrameReader _reader;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private PreviewImages _previewImages;
    [SerializeField] private bool _autoRead;
    [SerializeField] private Vector3 _addRotate;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow) || _autoRead)
        {
            if(_reader.GetFrame(out DataFrame frame))
            {
                _gameObject.transform.position = frame.Position;

                
                _gameObject.transform.rotation = frame.Rotation;
                _gameObject.transform.forward = -_gameObject.transform.forward;



                _previewImages.UpdatePreviews(frame);
            }
        }
    }
}
