using System;

public static class ProjectCore
{
    public static string Name { get; private set; }
    public static DateTime CreateDate { get; private set; }
    public static DateTime UpdateDate { get; private set; }
    public static string Model { get; private set; }

    public static void CreateProject(string name, DateTime createDate, DateTime updateDate, string model)
    {
        Name = name;
        CreateDate = SetDate(CreateDate, createDate);
        UpdateDate = SetDate(UpdateDate, updateDate);
        Model = model;
    }
    public static void SelectModel(string model)
    {
        Model = model;
    }
    public static void UpdateProject()
    {
        UpdateDate = DateTime.Now;
    }
    private static DateTime SetDate(DateTime targetDate, DateTime sourceDate)
    {
        if (sourceDate == default)
            return targetDate = DateTime.Now;
        else
            return targetDate = sourceDate;
    }
}
