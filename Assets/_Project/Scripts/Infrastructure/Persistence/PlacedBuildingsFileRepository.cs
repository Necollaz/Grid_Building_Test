using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;

namespace _Project.Scripts.Infrastructure.Persistence
{
    public class PlacedBuildingsFileRepository
    {
        private const Formatting JSON_FORMATTING = Formatting.Indented;
        private const string SAVE_FILE_NAME = "placed_buildings.json";

        private readonly IReadOnlyList<PlacedBuildingRecord> empty = Array.Empty<PlacedBuildingRecord>();
        private readonly string saveFilePath;

        [Inject]
        public PlacedBuildingsFileRepository()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }

        public IReadOnlyList<PlacedBuildingRecord> LoadPlacedBuildings()
        {
            if (!File.Exists(saveFilePath))
                return empty;

            try
            {
                string json = File.ReadAllText(saveFilePath);
                PlacedBuildingsWrapper wrapper = JsonConvert.DeserializeObject<PlacedBuildingsWrapper>(json);
                
                return (wrapper != null && wrapper.Placed != null) ? wrapper.Placed : empty;
            }
            catch (Exception _)
            {
                return empty;
            }
        }

        public IReadOnlyList<PlacedBuildingRecord> ImportPlacedBuildingsFromJson(string json)
        {
            try
            {
                PlacedBuildingsWrapper wrapper = JsonConvert.DeserializeObject<PlacedBuildingsWrapper>(json);
                
                return (wrapper != null && wrapper.Placed != null) ? wrapper.Placed : empty;
            }
            catch (Exception _)
            {
                return empty;
            }
        }
        
        public string ExportPlacedBuildingsToJson(IReadOnlyList<PlacedBuildingRecord> records)
        {
            PlacedBuildingsWrapper wrapper = new PlacedBuildingsWrapper
            {
                Placed = new List<PlacedBuildingRecord>(records)
            };

            return JsonConvert.SerializeObject(wrapper, JSON_FORMATTING);
        }
        
        public void SavePlacedBuildings(IReadOnlyList<PlacedBuildingRecord> records)
        {
            try
            {
                var wrapper = new PlacedBuildingsWrapper
                {
                    Placed = new List<PlacedBuildingRecord>(records)
                };
                
                string json = JsonConvert.SerializeObject(wrapper, JSON_FORMATTING);
                File.WriteAllText(saveFilePath, json);
            }
            catch (Exception _)
            {
            }
        }
    }
}