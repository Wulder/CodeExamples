using UnityEngine;

public class SetModel : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Set model angle");
        ButtonsDelegates.OnSetWall?.Invoke();
    }
}
