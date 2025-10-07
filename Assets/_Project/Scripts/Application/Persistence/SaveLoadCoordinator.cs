using System.Collections.Generic;
using UnityEngine;
using Zenject;
using _Project.Scripts.Application.Placement;
using _Project.Scripts.Domain.Grid;
using _Project.Scripts.Domain.Buildings;
using _Project.Scripts.Infrastructure.Config;
using _Project.Scripts.Infrastructure.Persistence;
using _Project.Scripts.Presentation.Views;

namespace _Project.Scripts.Application.Persistence
{
    public class SaveLoadCoordinator
    {
        private readonly PlacedBuildingsFileRepository repository;
        private readonly JsonBuildingCatalogProvider catalogProvider;
        private readonly GridOccupancyMap occupancy;
        private readonly PlacedBuildingInstanceRegistry instances;
        private readonly Transform worldRoot;
        private readonly BuildingFactory factory;

        [Inject]
        public SaveLoadCoordinator(PlacedBuildingsFileRepository repository, JsonBuildingCatalogProvider catalogProvider,
            GridOccupancyMap occupancy, PlacedBuildingInstanceRegistry instances, [Inject(Id = "WorldRoot")] Transform worldRoot,
            BuildingFactory factory)
        {
            this.repository = repository;
            this.catalogProvider = catalogProvider;
            this.occupancy = occupancy;
            this.instances = instances;
            this.worldRoot = worldRoot;
            this.factory = factory;
        }
        
        public string ExportJson()
        {
            var records = BuildRecordsFromInstances();
            
            return repository.ExportPlacedBuildingsToJson(records);
        }

        public void SaveCurrent()
        {
            var records = BuildRecordsFromInstances();

            repository.SavePlacedBuildings(records);
        }

        public void LoadFromDisk()
        {
            instances.ClearAll();
            
            var records = repository.LoadPlacedBuildings();
            
            RebuildWorldFromRecords(records);
        }

        public void ImportJsonAndRebuild(string json)
        {
            instances.ClearAll();
            
            if (string.IsNullOrWhiteSpace(json))
                return;

            var records = repository.ImportPlacedBuildingsFromJson(json);
            
            RebuildWorldFromRecords(records);
        }
        
        private List<PlacedBuildingRecord> BuildRecordsFromInstances()
        {
            var list = new List<PlacedBuildingRecord>();
            
            foreach (var pair in instances.Enumerate())
            {
                BuildingView view = pair.Value;
                list.Add(MapViewToRecord(view));
            }
            
            return list;
        }

        private PlacedBuildingRecord MapViewToRecord(BuildingView view)
        {
            return new PlacedBuildingRecord
            {
                BuildingId = view.BuildingId,
                GridX = view.GridPosition.x,
                GridY = view.GridPosition.y,
                RotationDegrees = view.RotationDegrees
            };
        }

        private void RebuildWorldFromRecords(IEnumerable<PlacedBuildingRecord> records)
        {
            BuildingCatalog catalog = catalogProvider.TryGetCatalog();
            
            foreach (PlacedBuildingRecord record in records)
            {
                if (!catalog.TryGet(record.BuildingId, out BuildingDefinition def))
                {
                    continue;
                }

                Sprite sprite = Resources.Load<Sprite>(def.SpriteResourcePath);
                
                if (sprite == null)
                {
                    continue;
                }

                Vector2Int grid = new Vector2Int(record.GridX, record.GridY);
                BuildingView view = factory.Create(def, sprite, grid, record.RotationDegrees, worldRoot);
                string instanceId = instances.Register(view);
                Vector2Int oriented = def.CalculateFootprint(record.RotationDegrees);
                occupancy.OccupyArea(grid, oriented, instanceId);
            }
        }
    }
}