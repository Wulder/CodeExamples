using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCutterUI : MonoBehaviour
{
    private RectangleHoleTool _holeTool;

    [SerializeField] private GameObject _editor;
    public void Init(RectangleHoleTool holeTool)
    {
        _holeTool = holeTool;
        _holeTool.OnSelectMesh += ShowEditor;
    }



    private void OnDisable()
    {
        _holeTool.OnSelectMesh -= ShowEditor;
    }
    void Start()
    {
        HideEditor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowEditor()
    {
        _editor.SetActive(true);
    }

    public void HideEditor()
    {
        _editor.SetActive(false);
    }

    public void CutHole()
    {
        _holeTool.CutHole();
    }

    #region SetSubtractorParams
    public void SetSizeX(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _holeTool.UpdateHoleSize(new Vector2(value, _holeTool.SelectedHole.Size.y));
        }
    }

    public void SetSizeY(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _holeTool.UpdateHoleSize(new Vector2(_holeTool.SelectedHole.Size.x, value));
        }
    }

    public void SetPosX(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _holeTool.UpdateHolePos(new Vector2(value, _holeTool.SelectedHole.Position.y));
        }
    }

    public void SetPosY(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _holeTool.UpdateHolePos(new Vector2(_holeTool.SelectedHole.Position.x, value));
        }
    }
    #endregion
}
