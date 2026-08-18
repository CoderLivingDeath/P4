using System;
using System.Collections.Generic;
using Project.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class DinoAllocationUI : ContentUi, IInitializable, IDisposable
{
    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _confirmButton;

    [SerializeField]
    private Image _dinoImage;

    [SerializeField]
    private TextMeshProUGUI _dinoTypeText;

    [SerializeField]
    private RectTransform _choiceButtonsRoot;

    [SerializeField]
    private RectTransform _infoPanelRoot;

    [SerializeField]
    private DinoStatInfoList _statInfoList;

    [SerializeField]
    private Color _selectedColor = new Color(1f, 0.84f, 0.6f);

    private DinoModel _currentDino;

    private Button _selectedButton;

    private TransferReason _selectedReason;

    private UnityAction _onConfirmClicked;

    private readonly List<DinoInfoPanelSlotUI> _infoSlots = new List<DinoInfoPanelSlotUI>();

    [Inject]
    private DinoQueueService _dinoQueueService;

    [Inject]
    private CameraController _cameraController;

    [Inject]
    private ResourcesManager _resourcesManager;

    [Inject]
    private DinoFoodSettings _dinoFoodSettings;

    [Inject]
    private VillageGuardSpawner _villageGuardSpawner;

    public void Initialize()
    {
        Hide();

        if (_closeButton != null)
            _closeButton.onClick.AddListener(Hide);

        _onConfirmClicked = () => ConfirmTransfer(_selectedReason);
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(_onConfirmClicked);

        if (_choiceButtonsRoot != null)
        {
            foreach (Transform child in _choiceButtonsRoot)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                    button.onClick.AddListener(() => SelectTarget(button));
            }
        }

        _infoSlots.Clear();
        if (_infoPanelRoot != null)
        {
            foreach (Transform child in _infoPanelRoot)
            {
                DinoInfoPanelSlotUI slot = child.GetComponent<DinoInfoPanelSlotUI>();
                if (slot != null)
                    _infoSlots.Add(slot);
            }
        }

        if (_infoSlots.Count == 0)
            _infoSlots.AddRange(GetComponentsInChildren<DinoInfoPanelSlotUI>(true));

        _cameraController.LocationChanged += OnLocationChanged;
    }

    public void Dispose()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(Hide);
        if (_confirmButton != null && _onConfirmClicked != null)
            _confirmButton.onClick.RemoveListener(_onConfirmClicked);
        _cameraController.LocationChanged -= OnLocationChanged;
    }

    public void Open(DinoModel dino)
    {
        ResetSelection();
        _currentDino = dino;

        if (_dinoImage != null && dino != null)
            _dinoImage.sprite = dino.Sprite;
        if (_dinoTypeText != null && dino != null)
            _dinoTypeText.text = dino.Type.ToString();

        RefreshStats(dino);

        Show();
    }

    private void RefreshStats(DinoModel dino)
    {
        foreach (DinoInfoPanelSlotUI slot in _infoSlots)
        {
            if (slot == null)
                continue;

            if (dino == null || slot.StatType == StatType.None)
            {
                slot.Clear();
                continue;
            }

            DinoStatInfo info = _statInfoList != null ? _statInfoList.Get(slot.StatType) : null;
            slot.Set(info, dino.GetStatValue(slot.StatType));
        }
    }

    private void SelectTarget(Button button)
    {
        if (_selectedButton != null && _selectedButton.targetGraphic != null)
            _selectedButton.targetGraphic.color = Color.white;

        _selectedButton = button;
        _selectedReason = ResolveTransferReason(button);

        if (_selectedButton != null && _selectedButton.targetGraphic != null)
            _selectedButton.targetGraphic.color = _selectedColor;
    }

    private TransferReason ResolveTransferReason(Button button)
    {
        TextMeshProUGUI label = button != null ? button.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (label != null && Enum.TryParse(label.text, out TransferReason reason))
            return reason;

        return TransferReason.Farm;
    }

    private void ConfirmTransfer(TransferReason reason)
    {
        if (_currentDino != null)
        {
            if (reason == TransferReason.Food)
            {
                int food = _dinoFoodSettings != null ? _dinoFoodSettings.ConvertToFood(_currentDino) : 0;
                if (_resourcesManager != null)
                    _resourcesManager.AddFood(food);

                Debug.Log(string.Format("Transfer fact: {0} ({1}) -> reason: {2} | food: +{3}",
                    _currentDino.Name, _currentDino.Type, reason, food));
            }
            if (reason == TransferReason.Defense)
            {
                if (_villageGuardSpawner != null)
                    _villageGuardSpawner.SpawnGuard(_currentDino);

                Debug.Log(string.Format("Transfer fact: {0} ({1}) -> reason: {2}",
                    _currentDino.Name, _currentDino.Type, reason));
            }
            else
            {
                Debug.Log(string.Format("Transfer fact: {0} ({1}) -> reason: {2}",
                    _currentDino.Name, _currentDino.Type, reason));
            }

            _dinoQueueService.Remove(_currentDino);
            _currentDino = null;
        }

        ResetSelection();
        Hide();
    }

    private void ResetSelection()
    {
        if (_selectedButton != null && _selectedButton.targetGraphic != null)
            _selectedButton.targetGraphic.color = Color.white;

        _selectedButton = null;
        _selectedReason = TransferReason.Farm;
    }

    private void OnLocationChanged(Location location)
    {
        if (location != Location.Village)
            Hide();
    }
}