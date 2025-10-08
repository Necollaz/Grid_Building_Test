using UnityEngine;
using Zenject;
using ProjectGame.Scripts.Application.Persistence;
using ProjectGame.Scripts.Application.Placement;
using ProjectGame.Scripts.Domain.Grid;
using ProjectGame.Scripts.Infrastructure.Input;
using ProjectGame.Scripts.Presentation.Views;

public class PresentationInstaller : MonoInstaller
{
    private const float CELL_SIZE_UNITS = 1f;
    
    [SerializeField] private Transform _worldRoot;
    [SerializeField] private BuildingView _buildingViewPrefab;

    public override void InstallBindings()
    {
        Container.BindInstance(_worldRoot).WithId("WorldRoot").AsSingle();
        Container.BindInstance(_buildingViewPrefab).AsSingle();

        Container.Bind<GridCoordinateConverter>().AsSingle().WithArguments(CELL_SIZE_UNITS, Vector2.zero);
        Container.Bind<GridOccupancyMap>().AsSingle();
        Container.Bind<PlacedBuildingInstanceRegistry>().AsSingle();

        Container.Bind<BuildingFactory>().AsSingle();
        Container.Bind<PlacementApplicationService>().AsSingle();
        Container.Bind<DeletionApplicationService>().AsSingle();
        Container.Bind<SaveLoadCoordinator>().AsSingle();
        Container.Bind<PlacementInputOrchestrator>().AsSingle();
    }
}