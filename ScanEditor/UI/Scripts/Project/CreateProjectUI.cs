using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateProjectUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField newProjectName;
    [SerializeField] private TMP_InputField fileForOpen;
    [SerializeField] private Button createBtn;
    [SerializeField] private Button cancelBtn;

    void Start()
    {
        WindowsController.createProjectWindow = gameObject;

        if (newProjectName == null || fileForOpen == null)
        {
            Debug.LogWarning("Меню создания нового проекта не сконфигурировано!");
            Application.Quit();
        }

        createBtn?.onClick.AddListener(() => CreateNewProject());
        cancelBtn?.onClick.AddListener(() => CancelCreateProject());

        gameObject.SetActive(false);
    }

    private void CancelCreateProject()
    {
        if (!string.IsNullOrEmpty(newProjectName.text))
        {
            newProjectName.text = string.Empty;
        }
        if (!string.IsNullOrEmpty(fileForOpen.text))
        {
            fileForOpen.text = string.Empty;
        }

        gameObject.SetActive(false);
    }

    private void CreateNewProject()
    {
        if (string.IsNullOrEmpty(newProjectName.text))
        {
            Debug.LogWarning("Поле имя проекта должно быть заполнено!");
            return;
        }

        ActiveProject.InitProject(name: newProjectName.text, modelPath: fileForOpen.text, action: ProjectActions.Create);

        SceneManager.LoadScene("ProjectScene");
    }
}
