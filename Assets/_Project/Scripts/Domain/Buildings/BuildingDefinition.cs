using UnityEngine;

namespace _Project.Scripts.Domain.Buildings
{
    public class BuildingDefinition
    {
        private const int FULL_ROTATION_DEGREES = 360;
        private const int QUARTER_ROTATION_DEGREES = 90;
        private const int THREE_QUARTER_ROTATION_DEGREES = 270;
        
        private readonly int gridWidth;
        private readonly int gridHeight;
        
        public BuildingDefinition(string id, string displayName, int gridWidth, int gridHeight, string spriteResourcePath)
        {
            Id = id;
            DisplayName = displayName;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            SpriteResourcePath = spriteResourcePath;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string SpriteResourcePath { get; }
        
        public Vector2Int CalculateFootprint(int rotationDegrees)
        {
            int normalized = ((rotationDegrees % FULL_ROTATION_DEGREES) + FULL_ROTATION_DEGREES) % FULL_ROTATION_DEGREES;
            bool rotate = normalized == QUARTER_ROTATION_DEGREES || normalized == THREE_QUARTER_ROTATION_DEGREES;
            
            return rotate ? new Vector2Int(gridHeight, gridWidth) : new Vector2Int(gridWidth, gridHeight);
        }
    }
}