using UnityEngine;

public class OpenFile : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Select file to open");
        ButtonsDelegates.OnSelectFile?.Invoke();
    }
}
