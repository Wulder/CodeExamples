using UnityEngine;
using UnityEngine.Events;

public class ShowHideActionsTree : ButtonCore
{
    public UnityEvent OnHide, OnShow;

    [SerializeField] private GameObject actionTreePanel;
    [SerializeField] private RectTransform buttonImage;

    private void Start()
    {
        actionTreePanel?.SetActive(false);
    }
    protected override void OnPress()
    {
        if (actionTreePanel == null) return;
        
        if (!actionTreePanel.activeSelf)
        {
            actionTreePanel.SetActive(true);
            buttonImage.localScale = new Vector3(1f, -1f, 1f);
            OnShow?.Invoke();
        }
        else
        {
            actionTreePanel.SetActive(false);
            buttonImage.localScale = new Vector3(1f, 1f, 1f);
            OnHide?.Invoke();
        }       
    }
}
