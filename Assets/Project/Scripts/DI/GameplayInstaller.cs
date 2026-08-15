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
    private ResourcesManager _resourcesManager;

    [SerializeField]
    private FoodSettings _foodSettings;

    public override void InstallBindings()
    {
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
        Container.BindInterfacesAndSelfTo<VillageUI>().FromInstance(_villageUI).AsSingle();
        Container.BindInterfacesAndSelfTo<HuntUI>().FromInstance(_huntUI).AsSingle();
        Container.Bind<ResourcesManager>().FromInstance(_resourcesManager).AsSingle();
        Container.Bind<FoodSettings>().FromInstance(_foodSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<FoodService>().AsSingle();
    }
}