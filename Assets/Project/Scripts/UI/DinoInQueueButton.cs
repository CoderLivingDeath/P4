using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DinoInQueueButton : MonoBehaviour
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Button _button;

    public void Configure(DinoModel dino, Action onClick)
    {
        if (_icon != null && dino != null)
            _icon.sprite = dino.Sprite;

        if (_button != null && onClick != null)
            _button.onClick.AddListener(new UnityAction(onClick));
    }
}