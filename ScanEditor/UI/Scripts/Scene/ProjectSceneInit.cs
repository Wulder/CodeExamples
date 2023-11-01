using UnityEngine;

public class ProjectSceneInit : MonoBehaviour
{
    private GameObject coreModelObject;

    private void Awake()
    {
        coreModelObject = GameObject.FindGameObjectWithTag("ProjectCore");

        if (coreModelObject != null)
        {
            coreModelObject.name = ProjectCore.Name;
        }
    }
}
