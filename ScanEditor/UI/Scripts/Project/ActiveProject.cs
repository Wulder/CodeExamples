using System;

public static class ActiveProject
{
    public static void InitProject(string name, string modelPath, ProjectActions action, DateTime createDate = default, DateTime updateDate = default)
    {
        if (action == ProjectActions.Create || action == ProjectActions.Open)
        {          
            ProjectCore.CreateProject(name, createDate, updateDate, modelPath);
        }
    }
}
