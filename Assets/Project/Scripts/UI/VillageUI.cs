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
        _cameraController.LocationArrived += OnLocationArrived;

        if (_cameraController.CurrentLocation == Location.Village)
            Show();
        else
            Hide();
    }

    public void Dispose()
    {
        _cameraController.LocationChanged -= OnLocationChanged;
        _cameraController.LocationArrived -= OnLocationArrived;
        _mapButton.onClick.RemoveListener(MoveToMap);
        _farmButton.onClick.RemoveListener(MoveToFarm);
    }

    private void OnLocationChanged(Location location)
    {
        if (location != Location.Village)
            Hide();
    }

    private void OnLocationArrived(Location location)
    {
        if (location == Location.Village)
            Show();
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
