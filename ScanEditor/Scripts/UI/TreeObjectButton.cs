using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TreeObjectButton : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;

    public GameObject Target => _target;
    private void SetTarget(GameObject gm)
    {
        _target = gm;
        _text.text = _target.name;
    }

    public void Init(GameObject gm)
    {
        SetTarget(gm);
        _button.onClick.AddListener(() => ActionsTreeController.DrawTransformGizmos(gm));
    }
}
