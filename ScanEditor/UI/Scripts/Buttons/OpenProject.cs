using UnityEngine;

public class OpenProject : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Open project");
        ButtonsDelegates.OnSelectProject?.Invoke();
    }
}
