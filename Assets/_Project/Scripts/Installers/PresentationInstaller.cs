using UnityEngine;
using Zenject;
using _Project.Scripts.Application.Persistence;
using _Project.Scripts.Application.Placement;
using _Project.Scripts.Domain.Grid;
using _Project.Scripts.Infrastructure.Input;
using _Project.Scripts.Presentation.Views;

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