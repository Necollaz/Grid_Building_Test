using System;
using UnityEngine;
using Zenject;
using ProjectGame.Scripts.Domain.Grid;
using ProjectGame.Scripts.Presentation.Views;

namespace ProjectGame.Scripts.Application.Placement
{
    public class DeletionApplicationService
    {
        private readonly GridOccupancyMap occupancy;
        private readonly PlacedBuildingInstanceRegistry instances;

        [Inject]
        public DeletionApplicationService(GridOccupancyMap occupancy, PlacedBuildingInstanceRegistry instances)
        {
            this.occupancy = occupancy;
            this.instances = instances;
        }

        public bool TryDeleteAtCell(Vector2Int cell, Func<BuildingView, (Vector2Int position, Vector2Int orientedSize)>
            footprintResolver)
        {
            if (!occupancy.WasTryGetAnyInstanceAtCell(cell, out string instanceId))
                return false;
            
            if (!instances.TryGetInstance(instanceId, out BuildingView view))
                return false;

            var info = footprintResolver(view);
            occupancy.FreeArea(info.position, info.orientedSize);
            
            instances.Remove(instanceId);
            
            return true;
        }
    }
}