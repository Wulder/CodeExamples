using UnityEngine;

public class CutElement : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Put elements from model");
        ButtonsDelegates.OnPutModelPart?.Invoke();
    }
}
