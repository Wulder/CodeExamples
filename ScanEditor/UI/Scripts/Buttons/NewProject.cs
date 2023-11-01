using UnityEngine;

public class NewProject : ButtonCore
{
    GameObject createWindow;
    public NewProject()
    {
    }
    protected override void OnPress()
    {
        if (WindowsController.createProjectWindow == null)
            return;

        WindowsController.createProjectWindow.SetActive(true);
    }
}
