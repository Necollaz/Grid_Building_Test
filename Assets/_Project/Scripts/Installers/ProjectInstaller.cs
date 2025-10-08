using Zenject;
using ProjectGame.Scripts.Application.Placement;
using ProjectGame.Scripts.Infrastructure.Config;
using ProjectGame.Scripts.Infrastructure.Persistence;

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