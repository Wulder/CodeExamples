using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class HotKeyToolSwitcher : MonoBehaviour
{

    [SerializeField] private List<HotKeyTool> _hotKeys = new List<HotKeyTool>();

    void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                if (_hotKeys.Exists(k => k.Key == vKey))
                {
                    var hk = _hotKeys.Find(k => k.Key == vKey);
                    hk.SetTool();
                }
            }
        }
    }



    [Serializable]
    struct HotKeyTool
    {
        public KeyCode Key;
        public ToolType ToolType;
        public void SetTool()
        {
            switch (ToolType)
            {
                case ToolType.Measurer:
                    {
                        ApplicationController.Instance._toolManager.SetTool(new MeasurerTool());
                        break;
                    }
            }
        }
    }
}
