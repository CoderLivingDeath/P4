using System;
using _Project.Scripts.Utils;
using Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VillageUI : ContentUi, IInitializable, IDisposable
{
    [SerializeField]
    private Button _mapButton;

    [SerializeField]
    private Button _farmButton;

    [Inject]
    private CameraController _cameraController;

    public void Initialize()
    {
        _mapButton.onClick.AddListener(MoveToMap);
        _farmButton.onClick.AddListener(MoveToFarm);
        _cameraController.LocationChanged += OnLocationChanged;
        OnLocationChanged(_cameraController.CurrentLocation);
    }

    public void Dispose()
    {
        _cameraController.LocationChanged -= OnLocationChanged;
        _mapButton.onClick.RemoveListener(MoveToMap);
        _farmButton.onClick.RemoveListener(MoveToFarm);
    }

    private void OnLocationChanged(Location location)
    {
        if (location == Location.Village)
            Show();
        else
            Hide();
    }

    private void MoveToMap()
    {
        _cameraController.MoveTo(Location.Map);
    }

    private void MoveToFarm()
    {
        _cameraController.MoveTo(Location.Farm);
    }
}
