using System;

namespace _Project.Scripts.Infrastructure.Config
{
    [Serializable]
    public class BuildingItemDto
    {
        public string Id;
        public string DisplayName;
        public int GridWidth;
        public int GridHeight;
        public string SpriteResourcePath;
    }
}