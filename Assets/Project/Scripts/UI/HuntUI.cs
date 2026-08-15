using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HuntUI : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField]
    private Button _villageButton;

    [SerializeField]
    private Button _caveButton;

    [SerializeField]
    private Button _seaShoreButton;

    [SerializeField]
    private Button _forestButton;

    [SerializeField]
    private Button _mountainButton;

    [SerializeField]
    private Button _fireLakeButton;

    [Inject]
    private CameraController _cameraController;

    private Button[] _zoneButtons;

    public Button[] ZoneButtons => _zoneButtons;

    public void Initialize()
    {
        _zoneButtons = new[]
        {
            _caveButton,
            _seaShoreButton,
            _forestButton,
            _mountainButton,
            _fireLakeButton
        };

        _villageButton.onClick.AddListener(GoToVillage);
    }

    public void Dispose()
    {
        _villageButton.onClick.RemoveListener(GoToVillage);
    }

    private void GoToVillage()
    {
        _cameraController.MoveTo(Location.Village);
    }
}
