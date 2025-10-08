using System.Collections.Generic;
using UnityEngine;
using ProjectGame.Scripts.Presentation.Views;

namespace ProjectGame.Scripts.Application.Placement
{
    public class PlacedBuildingInstanceRegistry
    {
        private readonly Dictionary<string, BuildingView> instancesById = new Dictionary<string, BuildingView>();
        private readonly Dictionary<BuildingView, string> idsByInstance = new Dictionary<BuildingView, string>();
        
        private int _autoIncrementId;

        public string Register(BuildingView instance)
        {
            string id = $"instance_{_autoIncrementId++}";
            instancesById[id] = instance;
            idsByInstance[instance] = id;
            
            return id;
        }

        public bool TryGetInstance(string id, out BuildingView view) => instancesById.TryGetValue(id, out view);

        public bool Remove(string id)
        {
            if (!instancesById.TryGetValue(id, out BuildingView view))
                return false;

            instancesById.Remove(id);
            idsByInstance.Remove(view);
            Object.Destroy(view.gameObject);
            
            return true;
        }
        
        public IEnumerable<KeyValuePair<string, BuildingView>> Enumerate() => instancesById;

        public void ClearAll()
        {
            foreach (BuildingView view in idsByInstance.Keys)
                Object.Destroy(view.gameObject);

            instancesById.Clear();
            idsByInstance.Clear();
        }
    }
}