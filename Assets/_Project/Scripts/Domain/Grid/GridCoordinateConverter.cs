using UnityEngine;

namespace ProjectGame.Scripts.Domain.Grid
{
    public class GridCoordinateConverter
    {
        private readonly Vector2 worldOrigin;
        private readonly float cellSizeUnits;

        public GridCoordinateConverter(float cellSizeUnits, Vector2 worldOrigin)
        {
            this.cellSizeUnits = Mathf.Max(0.001f, cellSizeUnits);
            this.worldOrigin = worldOrigin;
        }

        public float CellSizeUnits => cellSizeUnits;
        
        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            float x = (worldPosition.x - worldOrigin.x) / cellSizeUnits;
            float y = (worldPosition.y - worldOrigin.y) / cellSizeUnits;
            
            return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public Vector3 GridToWorld(Vector2Int gridPosition)
        {
            float x = worldOrigin.x + (gridPosition.x + 0.5f) * cellSizeUnits;
            float y = worldOrigin.y + (gridPosition.y + 0.5f) * cellSizeUnits;
            
            return new Vector3(x, y, 0f);
        }
    }
}