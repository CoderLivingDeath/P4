using System;
using UnityEngine;
using Zenject;

public class FoodService : IInitializable, ITickable, IDisposable
{
    private readonly ResourcesManager _resourcesManager;

    private readonly FoodSettings _settings;

    private float _drainTimer;

    [Inject]
    public FoodService(ResourcesManager resourcesManager, FoodSettings settings)
    {
        _resourcesManager = resourcesManager;
        _settings = settings;
    }

    public int Food => _resourcesManager.Food;

    public event Action<int> FoodChanged;

    public void Initialize()
    {
        _resourcesManager.SetFood(_settings.StartFood);
        _resourcesManager.FoodChanged += OnFoodChanged;
    }

    public void Tick()
    {
        if (_resourcesManager.Food <= 0)
            return;

        _drainTimer += Time.deltaTime;
        if (_drainTimer < _settings.DrainIntervalSeconds)
            return;

        _drainTimer = 0f;
        _resourcesManager.TryConsumeFood(_settings.DrainAmount);
    }

    public void Dispose()
    {
        _resourcesManager.FoodChanged -= OnFoodChanged;
    }

    public bool TrySpend(int amount)
    {
        return _resourcesManager.TryConsumeFood(amount);
    }

    private void OnFoodChanged(int value)
    {
        FoodChanged?.Invoke(value);
    }
}
