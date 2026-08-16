using System;
using UnityEngine;
using Zenject;

public enum MissionState
{
    Idle,
    Hunting,
}

public class MissionService : IInitializable, ITickable
{
    private readonly RewardRepository _rewardRepository;

    private readonly GameManager _gameManager;

    private MissionState _state;

    private HuntZone _zone;

    private float _remainingSeconds;

    [Inject]
    public MissionService(RewardRepository rewardRepository, GameManager gameManager)
    {
        _rewardRepository = rewardRepository;
        _gameManager = gameManager;
    }

    public MissionState State => _state;

    public HuntZone Zone => _zone;

    public int RemainingSeconds => Mathf.CeilToInt(_remainingSeconds);

    public event Action<HuntZone> MissionStarted;

    public event Action<int> MissionRemainingChanged;

    public event Action<HuntZone> MissionCompleted;

    public void Initialize()
    {
    }

    public bool StartMission(HuntZone zone)
    {
        if (_state != MissionState.Idle)
            return false;

        LocationReward entry = _rewardRepository.GetEntry(zone);
        if (entry == null)
            return false;

        _zone = zone;
        _remainingSeconds = entry.MissionTimeSeconds;
        _state = MissionState.Hunting;
        MissionStarted?.Invoke(_zone);
        MissionRemainingChanged?.Invoke(RemainingSeconds);
        return true;
    }

    public void Tick()
    {
        if (_state != MissionState.Hunting)
            return;

        if (_gameManager.IsGameOver)
            return;

        int previousSeconds = RemainingSeconds;
        _remainingSeconds -= Time.deltaTime;

        if (RemainingSeconds != previousSeconds)
            MissionRemainingChanged?.Invoke(RemainingSeconds);

        if (_remainingSeconds <= 0f)
        {
            _state = MissionState.Idle;
            MissionCompleted?.Invoke(_zone);
        }
    }
}