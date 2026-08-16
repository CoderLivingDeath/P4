using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField]
    private CameraController _cameraController;

    [SerializeField]
    private VillageUI _villageUI;

    [SerializeField]
    private HuntUI _huntUI;

    [SerializeField]
    private HuntPrepareUI _huntPrepareUI;

    [SerializeField]
    private ResourcesManager _resourcesManager;

    [SerializeField]
    private FoodSettings _foodSettings;

    [SerializeField]
    private PeopleSettings _peopleSettings;

    [SerializeField]
    private HungerSettings _hungerSettings;

    [SerializeField]
    private LocationSettings _locationSettings;

    [SerializeField]
    private RewardRepository _rewardRepository;

    [SerializeField]
    private GameOverUI _gameOverUI;

    public override void InstallBindings()
    {
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
        Container.BindInterfacesAndSelfTo<VillageUI>().FromInstance(_villageUI).AsSingle();
        Container.BindInterfacesAndSelfTo<HuntUI>().FromInstance(_huntUI).AsSingle();
        Container.BindInterfacesAndSelfTo<HuntPrepareUI>().FromInstance(_huntPrepareUI).AsSingle();
        Container.Bind<ResourcesManager>().FromInstance(_resourcesManager).AsSingle();
        Container.Bind<FoodSettings>().FromInstance(_foodSettings).AsSingle();
        Container.Bind<PeopleSettings>().FromInstance(_peopleSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<FoodService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PeopleService>().AsSingle();
        Container.Bind<HungerSettings>().FromInstance(_hungerSettings).AsSingle();
        Container.Bind<LocationSettings>().FromInstance(_locationSettings).AsSingle();
        Container.Bind<RewardRepository>().FromInstance(_rewardRepository).AsSingle();
        Container.BindInterfacesAndSelfTo<GameOverUI>().FromInstance(_gameOverUI).AsSingle();
        Container.BindInterfacesAndSelfTo<HungerService>().AsSingle();
        Container.Bind<GameManager>().AsSingle();
    }
}