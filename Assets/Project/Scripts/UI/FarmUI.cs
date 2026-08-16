using System;
using _Project.Scripts.Utils;
using Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FarmUI : ContentUi
{
    [SerializeField]
    private Button _villageButton;

    [Inject]
    private CameraController _cameraController;

    void OnEnable()
    {
        _villageButton.onClick.AddListener(MoveToVilage);
        _cameraController.LocationChanged += OnLocationChanged;
        OnLocationChanged(_cameraController.CurrentLocation);
    }

    void OnDisable()
    {
        _cameraController.LocationChanged -= OnLocationChanged;
        _villageButton.onClick.RemoveListener(MoveToVilage);
    }

    private void OnLocationChanged(Location location)
    {
        if (location == Location.Farm)
            Show();
        else
            Hide();
    }

    private void MoveToVilage()
    {
        _cameraController.MoveTo(Location.Village);
    }
}
