public class ButtonsDelegates
{
    //����� ���� ������ ����� ��� ��������
    public delegate void SelectFile();
    public static SelectFile OnSelectFile;

    //����� ������ ������������ ����
    public delegate void SetFloor();
    public static SetFloor OnSetFloor;

    //����� ������ ��������� ���� ������
    public delegate void SetModelAngle();
    public static SetModelAngle OnSetWall;

    //����� ������ ��������� ����� ������
    public delegate void PutModelPart();
    public static PutModelPart OnPutModelPart;

    //����� ������ ��������� ������ �� ������ (���/�������)
    public delegate void CutWalls();
    public static CutWalls OnCutWalls;

    //����� ������ �������� ����� ������
    public delegate void CreateModelPlan();
    public static CreateModelPlan OnCreateModelPlan;

    //����� ������ ���������� �������
    public delegate void SaveProject();
    public static SaveProject OnSaveProject;

    public delegate void SelectProject();
    public static SelectProject OnSelectProject;
}
