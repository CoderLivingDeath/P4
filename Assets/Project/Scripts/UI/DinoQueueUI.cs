using System;
using UnityEngine;
using Zenject;

public class DinoQueueUI : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField]
    private DinoInQueueButton _buttonPrefab;

    [SerializeField]
    private RectTransform _container;

    [Inject]
    private DinoQueueService _dinoQueueService;

    [Inject]
    private DinoAllocationUI _dinoAllocationUI;

    public void Initialize()
    {
        _dinoQueueService.Changed += Rebuild;
        Rebuild();
    }

    public void Dispose()
    {
        _dinoQueueService.Changed -= Rebuild;
    }

    private void Rebuild()
    {
        for (int i = _container.childCount - 1; i >= 0; i--)
            Destroy(_container.GetChild(i).gameObject);

        foreach (DinoModel dino in _dinoQueueService.Dinos)
        {
            if (_buttonPrefab == null)
                break;

            DinoInQueueButton button = Instantiate(_buttonPrefab, _container);
            button.Configure(dino, () => _dinoAllocationUI.Open(dino));
        }
    }
}