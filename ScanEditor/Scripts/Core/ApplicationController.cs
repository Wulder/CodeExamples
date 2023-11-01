using System;
using System.Collections;
using System.Collections.Generic;
using TriLibCore.Dae.Schema;
using UnityEngine;
using static ButtonsDelegates;

public class ApplicationController : MonoBehaviour
{

    //SINGLETON vars --------------------------------------------------
    private static ApplicationController instance;
    public static ApplicationController Instance => instance;
    //---------------------------------------------------------------------------

    public delegate void StartDel();
    public delegate void UpdateDel();
    public delegate void DrawGizmosDel();
    public delegate void GUIDel();

    StartDel _startDel;
    UpdateDel _updateCycle;
    DrawGizmosDel _gizmosCycle;
    GUIDel _guiCycle;

    public ToolManager _toolManager { get; private set; }
    private ModelLoader _modelLoader;
    public Camera MainCamera => _mainCamera;

    [SerializeField] private GameObject _mainMesh;
    [SerializeField] private AppConfig _appConfig;
    [SerializeField] private Camera _mainCamera;


    public GameObject MainMesh => _mainMesh;
    public AppConfig AppConfig => _appConfig;

    private void OnEnable()
    {
        ButtonsDelegates.OnSelectFile += _modelLoader.OpenFilePicker;
        ButtonsDelegates.OnSetFloor += _toolManager.SetFloor;
        ButtonsDelegates.OnSetWall += _toolManager.SetAngle;
        ButtonsDelegates.OnCutWalls += _toolManager.SetCutWalls;
        ButtonsDelegates.OnPutModelPart += _toolManager.SetSubtractorTool;
        ButtonsDelegates.OnCreateModelPlan += _toolManager.SetModelPlanTool;
    }

    private void OnDisable()
    {
        ButtonsDelegates.OnSelectFile -= _modelLoader.OpenFilePicker;
        ButtonsDelegates.OnSetFloor -= _toolManager.SetFloor;
        ButtonsDelegates.OnSetWall -= _toolManager.SetAngle;
        ButtonsDelegates.OnCutWalls -= _toolManager.SetCutWalls;
        ButtonsDelegates.OnPutModelPart -= _toolManager.SetSubtractorTool;
        ButtonsDelegates.OnCreateModelPlan -= _toolManager.SetModelPlanTool;
    }


    #region Base monobehaviour functions
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("Instance of ApplicationController already exist");
            Destroy(this);
        }

        _toolManager = new ToolManager(this);
        _modelLoader = new ModelLoader(this,ProjectCore.Model);

        if (!string.IsNullOrEmpty(ProjectCore.Model))
        {
            _modelLoader.LoadModel();
        }

        if(_mainMesh != null)
        {
            ReplaceMaterial();
        }
        
           
    }
    void Start()
    {
        _startDel?.Invoke();
    }
    void Update()
    {
        _updateCycle?.Invoke();
    }

    
    void OnDrawGizmos()
    {
        
        _gizmosCycle?.Invoke();
    }

    void OnGUI()
    {
        
        _guiCycle?.Invoke();
    }
    #endregion

    public void SubscribeOnUpdate(UpdateDel del)
    {
        _updateCycle += del;
    }

    public void SubscribeOnGizmos(DrawGizmosDel del)
    {
        _gizmosCycle += del;
    }

    public void SubscribeOnGUI(GUIDel del)
    {
        _guiCycle += del;
    }

    public void SetMainMesh(GameObject mesh)
    {
        Destroy(_mainMesh.gameObject);
        _mainMesh = mesh;
        ActionsTreeController.AddObject(mesh);

    }

    [ContextMenu("ReplaceShader")]
    public void ReplaceMaterial()
    {
        foreach(var mrs in _mainMesh.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (var material in mrs.materials)
            {
                Texture tex = material.mainTexture;

                Shader shader = Shader.Find("Shader Graphs/TwoDirectionSlice");

                material.shader = shader;
                material.SetTexture("_Texture", tex);
                material.SetFloat("_UpThreshold", 100);
                material.SetFloat("_DownThreshold", -100);
            }
        }

    }
}
