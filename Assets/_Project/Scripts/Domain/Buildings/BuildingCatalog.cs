using System.Collections.Generic;

namespace _Project.Scripts.Domain.Buildings
{
    public class BuildingCatalog
    {
        private readonly Dictionary<string, BuildingDefinition> definitionsById;

        public BuildingCatalog(IEnumerable<BuildingDefinition> definitions)
        {
            definitionsById = new Dictionary<string, BuildingDefinition>();
            
            foreach (BuildingDefinition definition in definitions)
                definitionsById[definition.Id] = definition;
        }

        public bool TryGet(string id, out BuildingDefinition definition) => definitionsById.TryGetValue(id, out definition);
        
        public IEnumerable<BuildingDefinition> EnumerateAll() => definitionsById.Values;
    }
}