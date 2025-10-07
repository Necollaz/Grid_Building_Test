using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using _Project.Scripts.Domain.Buildings;
using _Project.Scripts.Domain.Grid;

namespace _Project.Scripts.Presentation.Views
{
    public class BuildingFactory
    {
        private readonly BuildingView prefab;
        private readonly Func<Vector2Int, Vector3> gridToWorld;
        private readonly float cellSize;
        
        [Inject]
        public BuildingFactory(BuildingView buildingViewPrefab, GridCoordinateConverter converter)
        {
            prefab = buildingViewPrefab;
            gridToWorld = converter.GridToWorld;
            cellSize = converter.CellSizeUnits;
        }

        public BuildingView Create(BuildingDefinition definition, Sprite sprite, Vector2Int gridOrigin, int rotationDegrees, Transform parent)
        {
            Vector2Int footprint = definition.CalculateFootprint(rotationDegrees);

            BuildingView view = Object.Instantiate(prefab, parent);
            view.TrySetCellWorldSize(cellSize);
            view.Initialize(definition.Id, sprite, gridOrigin, rotationDegrees, footprint);
            view.TryApplyTransform(gridToWorld);
            
            return view;
        }
    }
}