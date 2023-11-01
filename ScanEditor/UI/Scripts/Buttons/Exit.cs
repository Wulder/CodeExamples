using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : ButtonCore
{
    protected override void OnPress()
    {
        Debug.Log("Exit");
        Application.Quit();
        
    }
}
