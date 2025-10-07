using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using _Project.Scripts.Domain.Buildings;

namespace _Project.Scripts.Infrastructure.Config
{
    public class JsonBuildingCatalogProvider
    {
        private const Formatting JSON_FORMATTING = Formatting.Indented;
        private const string STREAMING_RELATIVE_PATH = "buildings.json";
        private const string SPRITE_RESOURCE_PATH = "Sprites/Buildings/DefaultHouse";
        private const string DEFAULT_HOUSE_ID = "default_house";
        private const string DEFAULT_HOUSE_NAME = "Default House";
        private const int DEFAULT_GRID_WIDTH = 1;
        private const int DEFAULT_GRID_HEIGHT = 1;
        
        private BuildingCatalog _cached;

        public BuildingCatalog TryGetCatalog()
        {
            if (_cached != null)
                return _cached;

            string json = TryReadConfigJson();

            BuildingCatalogDto catalogDto = JsonConvert.DeserializeObject<BuildingCatalogDto>(json);
            List<BuildingDefinition> definitions = new List<BuildingDefinition>();

            if (catalogDto?.Buildings != null)
            {
                foreach (BuildingItemDto buildingItem in catalogDto.Buildings)
                {
                    definitions.Add(new BuildingDefinition(buildingItem.Id, buildingItem.DisplayName, 
                        buildingItem.GridWidth, buildingItem.GridHeight, buildingItem.SpriteResourcePath));
                }
            }

            _cached = new BuildingCatalog(definitions);

            return _cached;
        }

        private string TryReadConfigJson()
        {
            try
            {
                string streamingPath = Path.Combine(Application.streamingAssetsPath, STREAMING_RELATIVE_PATH);

                if (File.Exists(streamingPath))
                    return File.ReadAllText(streamingPath);

                return CreateDefaultJson();
            }
            catch (Exception _)
            {
                return CreateDefaultJson();
            }
        }

        private string CreateDefaultJson()
        {
            BuildingCatalogDto defaultCatalog = new BuildingCatalogDto
            {
                Buildings = new[]
                {
                    new BuildingItemDto
                    {
                        Id = DEFAULT_HOUSE_ID,
                        DisplayName = DEFAULT_HOUSE_NAME,
                        GridWidth = DEFAULT_GRID_WIDTH,
                        GridHeight = DEFAULT_GRID_HEIGHT,
                        SpriteResourcePath = SPRITE_RESOURCE_PATH
                    }
                }
            };

            return JsonConvert.SerializeObject(defaultCatalog, JSON_FORMATTING);
        }
    }
}