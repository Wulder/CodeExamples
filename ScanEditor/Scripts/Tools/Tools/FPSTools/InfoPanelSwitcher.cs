using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            _panel.SetActive(!_panel.active);
        }
    }
}
