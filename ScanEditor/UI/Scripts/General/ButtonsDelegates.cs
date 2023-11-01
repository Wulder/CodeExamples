public class ButtonsDelegates
{
    //Вызов окна выбора файла для загрузки
    public delegate void SelectFile();
    public static SelectFile OnSelectFile;

    //Вызов метода выравнивания пола
    public delegate void SetFloor();
    public static SetFloor OnSetFloor;

    //Вызов метода установки угла модели
    public delegate void SetModelAngle();
    public static SetModelAngle OnSetWall;

    //Вызов метода вырезания части модели
    public delegate void PutModelPart();
    public static PutModelPart OnPutModelPart;

    //Вызов метода обрезания модели по высоте (пол/потолок)
    public delegate void CutWalls();
    public static CutWalls OnCutWalls;

    //Вызов метода создания плана модели
    public delegate void CreateModelPlan();
    public static CreateModelPlan OnCreateModelPlan;

    //Вызов метода сохранения проекта
    public delegate void SaveProject();
    public static SaveProject OnSaveProject;

    public delegate void SelectProject();
    public static SelectProject OnSelectProject;
}
