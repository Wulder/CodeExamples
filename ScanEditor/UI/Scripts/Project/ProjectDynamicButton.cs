using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ProjectDynamicButton : MonoBehaviour
{
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(() => OnPress());
    }

    public void SetElementState(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
    }
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }
    private void OnPress()
    {
        Debug.Log("Button press");
    }
}
