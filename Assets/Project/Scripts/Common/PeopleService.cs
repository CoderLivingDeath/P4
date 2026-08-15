using UnityEngine;
using Zenject;

public class PeopleService : IInitializable, ITickable
{
    private readonly ResourcesManager _resourcesManager;

    private readonly HungerService _hungerService;

    private readonly GameManager _gameManager;

    private readonly PeopleSettings _settings;

    private float _growthTimer;

    [Inject]
    public PeopleService(ResourcesManager resourcesManager, HungerService hungerService, GameManager gameManager, PeopleSettings settings)
    {
        _resourcesManager = resourcesManager;
        _hungerService = hungerService;
        _gameManager = gameManager;
        _settings = settings;
    }

    public void Initialize()
    {
        _resourcesManager.SetPeople(_settings.StartPeople);
    }

    public void Tick()
    {
        if (_hungerService.State != HungerState.Normal || _gameManager.IsGameOver)
            return;

        _growthTimer += Time.deltaTime;
        if (_growthTimer < _settings.GrowthIntervalSeconds)
            return;

        _growthTimer = 0f;
        _resourcesManager.AddPeople(_settings.GrowthAmount);
    }
}
