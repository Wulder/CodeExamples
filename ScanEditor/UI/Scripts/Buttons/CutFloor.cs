using UnityEngine;

public class CutFloor : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Cutting walls");
        ButtonsDelegates.OnCutWalls?.Invoke();
    }
}
