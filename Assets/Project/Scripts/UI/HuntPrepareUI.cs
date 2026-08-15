using System;
using Project.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HuntPrepareUI : ContentUi, IInitializable, IDisposable
{
    [SerializeField]
    private TextMeshProUGUI _locationNameText;

    [SerializeField]
    private Button _closeButton;

    public void Initialize()
    {
        Hide();
        _closeButton.onClick.AddListener(Hide);
    }

    public void Dispose()
    {
        _closeButton.onClick.RemoveListener(Hide);
    }

    public void Open(string locationName)
    {
        _locationNameText.text = locationName;
        Show();
    }
}
