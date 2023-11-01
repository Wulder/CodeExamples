using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectanglePlanUI : MonoBehaviour
{
    [SerializeField] private RectanglesPlan _plan;
    [SerializeField] private RectangleMeshCreator _creator;
    [SerializeField] private RectanglesPlanController _controller;


    #region Tools

    public void SetRectanglesCreatorTool()
    {
        _controller.SetNewTool(new RectanglesCreatorTool(_plan));
    }

    public void SetRectangleRemoveTool()
    {
        _controller.SetNewTool(new RectangleRemoveTool(_plan));
    }

    public void SetHoleCutterTool()
    {
        _controller.SetNewTool(new RectangleHoleTool(_plan));
    }

    public void SetObjectsCreatorTool()
    {
        _controller.SetNewTool(new RectanglesObjectsCreatorTool(_plan));
    }

    #endregion

    #region HoleEdit


    public void HoleSetSizeX(string val)
    {
        float result;
        if (float.TryParse(val, out result))
        {
            _creator.UpdateHoleSize(new Vector2(result, _creator.Hole.Size.y));
        }
            
    }
    public void HoleSetSizeY(string val)
    {
        float result;
        if(float.TryParse(val, out result))
        {
            _creator.UpdateHoleSize(new Vector2(_creator.Hole.Size.x, result));
        }
        
    }
    public void HoleSetPosX(string val)
    {
        float result;
        if (float.TryParse(val, out result))
        {
            _creator.UpdateHolePos(new Vector2(result, _creator.Hole.Position.y));
        }
            
    }
    public void HoleSetPosY(string val)
    {
        float result;
        if (float.TryParse(val, out result))
        {
            _creator.UpdateHolePos(new Vector2(_creator.Hole.Position.x, result));
        }
            
    }
    public void PerformHole()
    {
        _creator.PerformHole();
    }
    #endregion
}
