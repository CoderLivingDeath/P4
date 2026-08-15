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

    public override void InstallBindings()
    {
        Container.Bind<CameraController>().FromInstance(_cameraController).AsSingle();
        Container.BindInterfacesAndSelfTo<VillageUI>().FromInstance(_villageUI).AsSingle();
        Container.BindInterfacesAndSelfTo<HuntUI>().FromInstance(_huntUI).AsSingle();
    }
}