using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonCore : MonoBehaviour, IButtonPress
{
    protected Button btn;
    private void Awake()
    {
        btn = GetComponent<Button>();
        btn?.onClick.AddListener(() => OnButtonPress());
    }
    public void OnButtonPress()
    {
        OnPress();
    }

    protected abstract void OnPress();
    public virtual void SetElementState(bool state)
    {
        gameObject.SetActive(state);
    }
}
