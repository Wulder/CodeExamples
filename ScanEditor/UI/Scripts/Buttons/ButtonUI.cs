using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour
{
    [SerializeField] private ProjectActions buttonType;

    Button btn;
    ButtonCore bc;

    void Start()
    {
        TryGetComponent(out btn);

        ButtonInit();
    }

    private void ButtonInit()
    {
        if (buttonType == ProjectActions.None) return;

        if (buttonType == ProjectActions.Create)
            bc = new NewProject();
        else if (buttonType == ProjectActions.Exit)
            bc = new Exit();
        else if (buttonType == ProjectActions.Open)
            bc = new OpenProject();

        btn.onClick.AddListener(() => bc?.OnButtonPress());
    }
}

public enum ProjectActions { None, Create = 1, Open = 11, Exit = 21 }