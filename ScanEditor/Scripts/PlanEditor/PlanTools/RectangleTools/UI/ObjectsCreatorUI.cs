using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsCreatorUI : MonoBehaviour
{

    private RectanglesObjectsCreatorTool _tool;
    public void SetObject(GameObject obj)
    {
        _tool.SetObject(obj);
    }

    public void Init(RectanglesObjectsCreatorTool tool)
    {
        _tool = tool;
    }

    public void SetXSize(string val)
    {
        float value;
        if(float.TryParse(val, out value))
        {
            _tool.SetObjectSize(new Vector2(value, _tool.ObjectsSize.y));
        }
    }
    public void SetYSize(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _tool.SetObjectSize(new Vector2(_tool.ObjectsSize.x, value));
        }
    }
    public void SetXOffset(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _tool.SetObjectPos(new Vector2(value, _tool.ObjectsPosition.y));
        }
    }
    public void SetYOffset(string val)
    {
        float value;
        if (float.TryParse(val, out value))
        {
            _tool.SetObjectPos(new Vector2(_tool.ObjectsPosition.x, value));
        }
    }
}
