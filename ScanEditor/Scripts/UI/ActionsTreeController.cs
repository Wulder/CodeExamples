using RuntimeHandle;
using UnityEngine;

public class ActionsTreeController : MonoBehaviour
{
    private static ActionsTreeController instance;

    [SerializeField] private Transform _content;
    private GameObject _currentGizmo;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public static void AddObject(GameObject gmObject)
    {
        Debug.Log("Added new object in tree");
        var buttonObjectPrefab = Resources.Load("UI/TreeObjectButton", typeof(GameObject)) as GameObject;
        var buttonObject = Instantiate(buttonObjectPrefab,instance._content);
        

        var treeObject = buttonObject.GetComponent<TreeObjectButton>();
        treeObject.Init(gmObject);
    }

    public static void DrawTransformGizmos(GameObject target)
    {
        if(instance._currentGizmo != null)
            Destroy(instance._currentGizmo);

        var gizmoPrefab = Resources.Load("Objects/GizmoController", typeof(GameObject)) as GameObject;
        var gizmo = Instantiate(gizmoPrefab);

        var handles = gizmo.GetComponents<RuntimeTransformHandle>();

        foreach (var handle in handles)
        {
            handle.target = target.transform;
        }
        instance._currentGizmo = gizmo;
    }

    public void HideGizmos()
    {
        if(_currentGizmo != null)
            Destroy(instance._currentGizmo);
    }

    public void ResetTool()
    {
        ApplicationController.Instance._toolManager.SetTool(new EmptyTool());
    }
}
