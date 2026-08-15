using System;
using UnityEngine;
using Zenject;

public enum HungerState
{
    Normal,
    Hungry,
    PostHunger,
    GameOver,
}

public class HungerService : IInitializable, ITickable, IDisposable
{
    private readonly FoodService _foodService;

    private readonly HungerSettings _settings;

    private readonly GameOverUI _gameOverUI;

    private HungerState _state;

    private float _hungerTimer;

    private float _postHungerTimer;

    [Inject]
    public HungerService(FoodService foodService, HungerSettings settings, GameOverUI gameOverUI)
    {
        _foodService = foodService;
        _settings = settings;
        _gameOverUI = gameOverUI;
    }

    public HungerState State => _state;

    public void Initialize()
    {
        _foodService.FoodChanged += OnFoodChanged;
        OnFoodChanged(_foodService.Food);
    }

    public void Tick()
    {
        switch (_state)
        {
            case HungerState.Hungry:
                _hungerTimer += Time.deltaTime;
                if (_hungerTimer < _settings.HungerDurationSeconds)
                    return;

                _state = HungerState.GameOver;
                _gameOverUI.Show();
                return;

            case HungerState.PostHunger:
                _postHungerTimer += Time.deltaTime;
                if (_postHungerTimer < _settings.PostHungerDurationSeconds)
                    return;

                _state = HungerState.Normal;
                Debug.Log("Постголод прекратился");
                return;
        }
    }

    public void Dispose()
    {
        _foodService.FoodChanged -= OnFoodChanged;
    }

    private void OnFoodChanged(int food)
    {
        if (food <= 0)
        {
            if (_state == HungerState.Normal || _state == HungerState.PostHunger)
            {
                _state = HungerState.Hungry;
                _hungerTimer = 0f;
                Debug.Log("Голод начался");
            }
        }
        else if (_state == HungerState.Hungry)
        {
            _state = HungerState.PostHunger;
            _hungerTimer = 0f;
            _postHungerTimer = 0f;
            Debug.Log("Голод прекратился");
            Debug.Log("Постголод начался");
        }
    }
}
