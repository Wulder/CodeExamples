using System.Collections;
using System.Collections.Generic;
using TriLibCore;
using UnityEngine;

public class ModelLoader 
{
    private string _path;
    private ApplicationController _appController;
    public ModelLoader(ApplicationController appController, string path)
    {
        _appController = appController;
        _path = path;
    }

    public void LoadModel()
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        AssetLoader.LoadModelFromFile(_path, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
    }

    public void OpenFilePicker()
    {
        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
    }
    private void OnBeginLoad(bool filesSelected)
    {
        
    }

    private void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    /// <summary>
    /// Called when the Model loading progress changes.
    /// </summary>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    /// <param name="progress">The loading progress.</param>
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    /// <summary>
    /// Called when the Model (including Textures and Materials) has been fully loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
        ConfigureMaterial(assetLoaderContext.RootGameObject.GetComponentInChildren<MeshRenderer>().gameObject);
    }

    /// <summary>
    /// Called when the Model Meshes and hierarchy are loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model loaded. Loading materials.");
        GameObject root = assetLoaderContext.RootGameObject;
        MeshRenderer mr = root.GetComponentInChildren<MeshRenderer>();
        mr.gameObject.transform.SetParent(null, false);
        GameObject.Destroy(root);
        ConfigureLoadedmesh(mr.gameObject);
        assetLoaderContext.RootGameObject = mr.gameObject;
        MeshRoot.AlignHeight(mr.gameObject);
        _appController.SetMainMesh(mr.gameObject);
    }

    private void ConfigureLoadedmesh(GameObject mesh)
    {
        mesh.AddComponent<MeshCollider>();

       
    }

    private void ConfigureMaterial(GameObject mesh)
    {
        MeshRenderer mr = mesh.GetComponent<MeshRenderer>();
        Shader shader = Shader.Find("Shader Graphs/TwoDirectionSlice");
        Texture tex = mr.material.mainTexture;

        mr.material.shader = shader;
        mr.material.SetTexture("_Texture", tex);
        mr.material.SetFloat("_UpThreshold", 10);
        mr.material.SetFloat("_DownThreshold", -10);
    }


}
