using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    private static string _imagesPath, _posesPath, _reconstructionFilePath;

    public static string ImagesPath => _imagesPath;
    public static string ReconstructionFilePath => _reconstructionFilePath;

    

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void SetImagesPath(string path) => _imagesPath = path;
    public void SetReconstructionFilePath(string path) => _reconstructionFilePath = path;
}
