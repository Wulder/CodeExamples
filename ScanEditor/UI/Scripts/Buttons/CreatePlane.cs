using UnityEngine;

public class CreatePlane : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Create model plane");
        ButtonsDelegates.OnCreateModelPlan?.Invoke();
    }
}
