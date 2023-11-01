using UnityEngine;

public class SetFloor : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Set floor on plane");
        ButtonsDelegates.OnSetFloor?.Invoke();
    }
}
