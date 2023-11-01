using UnityEngine;


public class ObjectCreator : MonoBehaviour
{

    public static GameObject CreateObjectFromMesh(Mesh mesh, Material mat, Vector3 pos, bool addInTree, string name = "submesh",  CreationParameters creationParams = new CreationParameters())
    {
        GameObject subMesh = new GameObject(name);
        subMesh.AddComponent<EscanObject>();
        subMesh.transform.position = pos;

        MeshFilter mFilter = subMesh.AddComponent<MeshFilter>();
        MeshRenderer mRenderer = subMesh.AddComponent<MeshRenderer>();
        mRenderer.material = mat;


        mFilter.mesh = mesh;

        mFilter.mesh.RecalculateNormals();
        BoxCollider bc = subMesh.AddComponent<BoxCollider>();


        if (creationParams.Highlighting)
        {
            // subMesh.AddComponent<OutlineController>();
        }

        if (addInTree)
            ActionsTreeController.AddObject(subMesh);

        return subMesh;
    }


}
public struct CreationParameters
{
    public bool Highlighting;
}

