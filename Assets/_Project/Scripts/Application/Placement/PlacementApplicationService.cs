using UnityEngine;
using Zenject;
using ProjectGame.Scripts.Domain.Buildings;
using ProjectGame.Scripts.Domain.Grid;
using ProjectGame.Scripts.Infrastructure.Persistence;
using ProjectGame.Scripts.Presentation.Views;

namespace ProjectGame.Scripts.Application.Placement
{
    public class PlacementApplicationService
    {
        private readonly GridCoordinateConverter converter;
        private readonly GridOccupancyMap occupancy;
        private readonly PlacedBuildingInstanceRegistry instances;
        private readonly Transform worldRoot;
        private readonly BuildingFactory factory;

        [Inject]
        public PlacementApplicationService(GridCoordinateConverter converter, GridOccupancyMap occupancy,
            PlacedBuildingInstanceRegistry instances, [Inject(Id = "WorldRoot")] Transform worldRoot,
            BuildingFactory factory)
        {
            this.converter = converter;
            this.occupancy = occupancy;
            this.instances = instances;
            this.worldRoot = worldRoot;
            this.factory = factory;
        }

        public bool TryPlace(BuildingDefinition definition, Sprite sprite, Vector2Int leftBottomGrid, int rotationDegrees,
            out PlacedBuildingRecord record, out BuildingView view)
        {
            view = null;
            record = default;

            Vector2Int orientedFootprint = definition.CalculateFootprint(rotationDegrees);
            
            if (!occupancy.IsAreaFree(leftBottomGrid, orientedFootprint))
                return false;
            
            view = factory.Create(definition, sprite, leftBottomGrid, rotationDegrees, worldRoot);

            string instanceId = instances.Register(view);
            occupancy.OccupyArea(leftBottomGrid, orientedFootprint, instanceId);

            record = new PlacedBuildingRecord
            {
                BuildingId = definition.Id,
                GridX = leftBottomGrid.x,
                GridY = leftBottomGrid.y,
                RotationDegrees = rotationDegrees
            };
            
            return true;
        }
    }
}