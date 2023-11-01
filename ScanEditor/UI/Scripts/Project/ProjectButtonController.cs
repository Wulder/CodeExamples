using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectButtonController : MonoBehaviour
{
    [Header("Navigation keys section")]
    [SerializeField] private Button prevControlls;
    [SerializeField] private Button nextControlls;
    [Header("Saving project section")]
    [SerializeField] private Button saveProject;
    [SerializeField] private int showIndex = 1;

    [Header("Keys layers section")]
    [SerializeField] private List<KeysBlock> keysBlocks = new List<KeysBlock>();

    private RectTransform prevBtnRect;
    private RectTransform nextBtnRect;

    int startId = 0;
    int currenScrId = 0;

    private void Start()
    {
        SetStartId();

        InitNavigationKeys();

        SetActiveButton();
    }

    private void InitNavigationKeys()
    {
        prevControlls?.onClick.AddListener(() => ActivatePrevKeyBlock());
        nextControlls?.onClick.AddListener(() => ActivateNextKeyBlock());

        prevBtnRect = prevControlls?.GetComponent<RectTransform>();
        nextBtnRect = nextControlls?.GetComponent<RectTransform>();
    }

    private void ActivateNextKeyBlock()
    {
        currenScrId++;
        if (currenScrId >= keysBlocks.Count - 1)
            currenScrId = keysBlocks.Count - 1;
        SetActiveButton();
    }

    private void ActivatePrevKeyBlock()
    {
        currenScrId--;
        if (currenScrId < 0)
            currenScrId = 0;
        SetActiveButton();
    }

    private void SetStartId()
    {
        if (!string.IsNullOrEmpty(ProjectCore.Model))
        {
            startId++;
            currenScrId = startId;
        }
    }

    private void SetActiveButton()
    {
        //Устанавливаем активный блок кнопок (при старте и при выборе нового набора на сцене)
        foreach (var item in keysBlocks)
        {
            if (item.blockIndex == currenScrId)
            {
                SetButtonState(item.buttons, true);
            }
            else
            {
                SetButtonState(item.buttons, false);
            }
        }

        if (currenScrId <= 0)
        {
            prevControlls.gameObject.SetActive(false);
        }
        else
        {
            prevControlls.gameObject.SetActive(true);
        }

        if (currenScrId >= keysBlocks.Count - 1)
        {
            nextControlls.gameObject.SetActive(false);
        }
        else
        {
            nextControlls.gameObject.SetActive(true);
        }


        if (currenScrId >= showIndex)
        {
            saveProject.gameObject.SetActive(true);
        }
        else 
        { 
            saveProject.gameObject.SetActive(false);
        }
    }

    private void SetButtonState(List<ButtonCore> buttons, bool state)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var item = buttons[i];
            item.SetElementState(state);

            if (state)
            {
                SetNavigationButtonPos(i, buttons.Count, item.transform);
            }

        }
    }

    private void SetNavigationButtonPos(int i, int lenght, Transform target)
    {
        if (i == 0)
        {
            SetPrevKeyPos(target);
        }

        if (i == lenght - 1)
        {
            SetNextKeyPos(target);
        }
    }

    private void SetPrevKeyPos(Transform firstElement)
    {
        var rect = firstElement.GetComponent<RectTransform>();
        prevBtnRect.anchoredPosition = new Vector3(rect.anchoredPosition.x - 267, rect.anchoredPosition.y, 0);
    }

    private void SetNextKeyPos(Transform lastElement)
    {
        var rect = lastElement.GetComponent<RectTransform>();
        nextBtnRect.anchoredPosition = new Vector3(rect.anchoredPosition.x + 267, rect.anchoredPosition.y, 0);
    }
}

[System.Serializable]
public class KeysBlock
{
    public int blockIndex;
    public List<ButtonCore> buttons = new List<ButtonCore>();

}
