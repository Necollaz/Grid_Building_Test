using Zenject;
using _Project.Scripts.Application.Placement;
using _Project.Scripts.Infrastructure.Config;
using _Project.Scripts.Infrastructure.Persistence;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlacedBuildingsFileRepository>().AsSingle();
        Container.Bind<JsonBuildingCatalogProvider>().AsSingle();
        Container.Bind<PlacementModeState>().AsSingle();
        Container.Bind<SelectedBuildingRegistry>().AsSingle();
    }
}