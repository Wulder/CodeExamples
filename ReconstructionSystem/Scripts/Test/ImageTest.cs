using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System;

public class ImageTest : MonoBehaviour
{

   [SerializeField] private string _imagePath = "D:\\DataSet\\datasets\\street1\\depth\\000800.png";
    [SerializeField] private string _imageOut = "C:\\Users\\simp\\Desktop\\out\\out.png";

    [SerializeField] private InterpolationFlags _interpolation;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))  
        {
            Doit();
        }
    }

    void Doit()
    {
        Debug.Log("-------------------action--------------------");
        Mat mat = Cv2.ImRead( _imagePath,ImreadModes.AnyDepth);

       short[] pixels = new short[256*196]; 
         
        //Cv2.Resize(mat, mat, new Size(640, 480), interpolation: _interpolation);

        
        mat.GetArray<short>(out pixels);

        Mat mat2 = new Mat(192,256,MatType.CV_16UC1);




        mat2.SetArray(pixels);

        mat2.SaveImage(_imageOut);



       
        
    }
}
