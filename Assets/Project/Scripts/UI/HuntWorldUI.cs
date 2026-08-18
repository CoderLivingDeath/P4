using System;
using System.Collections.Generic;
using Project.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HuntWorldUI : ContentUi, IInitializable, IDisposable
{
    [SerializeField]
    private TextMeshProUGUI _locationNameText;

    [SerializeField]
    private Button _closeButton;

    [SerializeField]
    private Button _sendButton;

    [SerializeField]
    private Button _villageButton;

    [SerializeField]
    private GameObject _preHuntUI;

    [SerializeField]
    private GameObject _onHuntUI;

    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private List<RewardSlotPanel> _slots = new List<RewardSlotPanel>();

    [SerializeField]
    private GameObject _postHuntUI;

    [SerializeField]
    private Button _claimButton;

    [SerializeField]
    private Button _huntOverButton;

    private HuntZone _currentZone;

    private string _currentLocationName;

    private LocationReward _currentEntry;

    private int _pendingFood;

    private readonly List<DinoModel> _caughtDinos = new List<DinoModel>();

    private bool _hasPendingReward;

    [Inject]
    private RewardRepository _rewardRepository;

    [Inject]
    private CameraController _cameraController;

    [Inject]
    private MissionService _missionService;

    [Inject]
    private ResourcesManager _resourcesManager;

    [Inject]
    private DinoQueueService _dinoQueueService;

    public void Initialize()
    {
        Hide();

        if (_sendButton == null && _preHuntUI != null)
            _sendButton = _preHuntUI.GetComponentInChildren<Button>();
        if (_villageButton == null && _onHuntUI != null)
            _villageButton = _onHuntUI.GetComponentInChildren<Button>();
        if (_claimButton == null && _postHuntUI != null)
            _claimButton = _postHuntUI.GetComponentInChildren<Button>();

        _closeButton.onClick.AddListener(Hide);
        if (_sendButton != null)
            _sendButton.onClick.AddListener(StartHunt);
        if (_villageButton != null)
            _villageButton.onClick.AddListener(GoToVillage);
        if (_claimButton != null)
            _claimButton.onClick.AddListener(Claim);
        if (_huntOverButton != null)
            _huntOverButton.onClick.AddListener(OnHuntOverClick);
        _cameraController.LocationChanged += OnLocationChanged;
        _cameraController.LocationArrived += OnLocationArrived;
        _missionService.MissionRemainingChanged += OnMissionRemainingChanged;
        _missionService.MissionCompleted += OnMissionCompleted;

        ToggleHuntOverButton(false);
    }

    public void Dispose()
    {
        _closeButton.onClick.RemoveListener(Hide);
        if (_sendButton != null)
            _sendButton.onClick.RemoveListener(StartHunt);
        if (_villageButton != null)
            _villageButton.onClick.RemoveListener(GoToVillage);
        if (_claimButton != null)
            _claimButton.onClick.RemoveListener(Claim);
        if (_huntOverButton != null)
            _huntOverButton.onClick.RemoveListener(OnHuntOverClick);
        _cameraController.LocationChanged -= OnLocationChanged;
        _cameraController.LocationArrived -= OnLocationArrived;
        _missionService.MissionRemainingChanged -= OnMissionRemainingChanged;
        _missionService.MissionCompleted -= OnMissionCompleted;
    }

    private void StartHunt()
    {
        if (!_missionService.StartMission(_currentZone))
            return;

        if (_preHuntUI != null)
            _preHuntUI.SetActive(false);
        if (_onHuntUI != null)
            _onHuntUI.SetActive(true);
    }

    private void GoToVillage()
    {
        Hide();
        _cameraController.MoveTo(Location.Village);
    }

    private void OnLocationChanged(Location location)
    {
        if (location == Location.Map)
            return;

        Hide();
        if (_hasPendingReward)
            ToggleHuntOverButton(true);
    }

    private void OnLocationArrived(Location location)
    {
        if (location != Location.Map)
            return;

        if (_hasPendingReward)
        {
            ShowPostHunt();
            return;
        }

        if (_missionService.State == MissionState.Hunting)
            Open(_missionService.Zone, _currentLocationName);
    }

    public void Open(HuntZone zone, string locationName)
    {
        _currentZone = zone;
        _currentLocationName = locationName;
        _locationNameText.text = locationName;

        if (_hasPendingReward)
        {
            ShowPostHunt();
            return;
        }

        LocationReward entry = _rewardRepository != null ? _rewardRepository.GetEntry(zone) : null;
        if (entry == null)
        {
            ClearSlots();
            Show();
            return;
        }

        int slotIndex = 0;

        SetSlot(slotIndex++, _rewardRepository != null ? _rewardRepository.TimeIcon : null, FormatTime(entry.MissionTimeSeconds), false);

        Reward food = entry.Food;
        string foodAmount = entry.FoodUseRange
            ? string.Format("{0}-{1}", entry.FoodMinQuantity, entry.FoodMaxQuantity)
            : entry.FoodQuantity.ToString();
        SetSlot(slotIndex++, food != null ? food.Icon : null, foodAmount, false);

        foreach (LocationDino locationDino in entry.Dinos)
        {
            Dino dino = locationDino != null ? locationDino.Dino : null;
            bool locked = locationDino != null && !locationDino.AvailableFromStart;

            string amount = locked
                ? "?"
                : dino != null
                    ? string.Format("{0} {1}%", dino.Name, Mathf.RoundToInt(locationDino.CatchChance * 100f))
                    : string.Empty;

            Sprite icon = locked
                ? (_rewardRepository != null ? _rewardRepository.UnknownRewardIcon : null)
                : dino != null ? dino.Sprite : null;

            SetSlot(slotIndex++, icon, amount, locked);
        }

        while (slotIndex < _slots.Count)
            SetSlot(slotIndex++, null, string.Empty, false);

        if (_missionService.State == MissionState.Hunting)
        {
            if (_preHuntUI != null)
                _preHuntUI.SetActive(false);
            if (_onHuntUI != null)
                _onHuntUI.SetActive(true);
            if (_timerText != null)
                _timerText.text = FormatTime(_missionService.RemainingSeconds);
        }

        Show();
    }

    private void OnMissionRemainingChanged(int seconds)
    {
        if (_timerText == null)
            return;

        _timerText.text = FormatTime(seconds);
    }

    private void OnMissionCompleted(HuntZone zone)
    {
        _currentEntry = _rewardRepository != null ? _rewardRepository.GetEntry(zone) : null;
        RollReward();
        ShowPostHunt();
    }

    private void RollReward()
    {
        _pendingFood = 0;
        _caughtDinos.Clear();

        if (_currentEntry != null)
        {
            _pendingFood = _currentEntry.FoodUseRange
                ? UnityEngine.Random.Range(_currentEntry.FoodMinQuantity, _currentEntry.FoodMaxQuantity + 1)
                : _currentEntry.FoodQuantity;

            foreach (LocationDino locationDino in _currentEntry.Dinos)
            {
                if (locationDino == null || !locationDino.AvailableFromStart || locationDino.Dino == null)
                    continue;

                if (UnityEngine.Random.value <= locationDino.CatchChance)
                    _caughtDinos.Add(DinoFactory.Create(locationDino.Dino));
            }
        }

        _hasPendingReward = true;
    }

    private void ShowPostHunt()
    {
        if (_preHuntUI != null)
            _preHuntUI.SetActive(false);
        if (_onHuntUI != null)
            _onHuntUI.SetActive(false);
        if (_postHuntUI != null)
            _postHuntUI.SetActive(true);

        FillRewardSlots();

        if (_cameraController != null && _cameraController.CurrentLocation == Location.Map)
        {
            Show();
            ToggleHuntOverButton(false);
        }
        else
        {
            Hide();
            ToggleHuntOverButton(true);
        }
    }

    private void FillRewardSlots()
    {
        int slotIndex = 0;

        if (_currentEntry != null)
        {
            SetSlot(slotIndex++, _rewardRepository != null ? _rewardRepository.TimeIcon : null, FormatTime(_currentEntry.MissionTimeSeconds), false);

            Reward food = _currentEntry.Food;
            string foodAmount = _pendingFood > 0 ? string.Format("+{0}", _pendingFood) : string.Empty;
            SetSlot(slotIndex++, food != null ? food.Icon : null, foodAmount, false);
        }
        else
        {
            SetSlot(slotIndex++, null, string.Empty, false);
            SetSlot(slotIndex++, null, string.Empty, false);
        }

        foreach (DinoModel dino in _caughtDinos)
        {
            if (slotIndex >= _slots.Count)
                break;

            SetSlot(slotIndex++, dino != null ? dino.Sprite : null, dino != null ? dino.Name : string.Empty, false);
        }

        while (slotIndex < _slots.Count)
            SetSlot(slotIndex++, null, string.Empty, false);
    }

    public void Claim()
    {
        if (_resourcesManager != null && _pendingFood > 0)
            _resourcesManager.AddFood(_pendingFood);

        foreach (DinoModel dino in _caughtDinos)
        {
            if (dino != null)
                _dinoQueueService.Add(dino);
        }

        _hasPendingReward = false;
        _pendingFood = 0;
        _caughtDinos.Clear();

        if (_preHuntUI != null)
            _preHuntUI.SetActive(true);
        if (_onHuntUI != null)
            _onHuntUI.SetActive(false);
        if (_postHuntUI != null)
            _postHuntUI.SetActive(false);

        ClearSlots();
        Hide();
        ToggleHuntOverButton(false);
    }

    private void OnHuntOverClick()
    {
        _cameraController.MoveTo(Location.Map);
    }

    private void ToggleHuntOverButton(bool active)
    {
        if (_huntOverButton != null)
            _huntOverButton.gameObject.SetActive(active);
    }

    private void SetSlot(int index, Sprite icon, string amount, bool locked)
    {
        if (index < 0 || index >= _slots.Count)
            return;

        _slots[index].Set(icon, amount, locked);
    }

    private void ClearSlots()
    {
        foreach (RewardSlotPanel slot in _slots)
            slot.Clear();
    }

    private static string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remaining = seconds % 60;
        return string.Format("{0}:{1:00}", minutes, remaining);
    }
}