using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ProjectGame.Scripts.Application.Placement;
using ProjectGame.Scripts.Domain.Buildings;
using ProjectGame.Scripts.Infrastructure.Config;

namespace ProjectGame.Scripts.Presentation.UI
{
    public class BuildingCatalogUiBuilder : MonoBehaviour
    {
        [SerializeField] private Transform _itemsRoot;
        [SerializeField] private BuildingItemView _itemPrefab;

        [Inject] private JsonBuildingCatalogProvider _catalogProvider;
        [Inject] private SelectedBuildingRegistry _selectedRegistry;

        private readonly List<BuildingItemView> spawnedItems = new List<BuildingItemView>();

        private void Start()
        {
            BuildFromCatalog();
            
            _selectedRegistry.SelectedBuildingChanged += OnSelectedChanged;
        }

        private void OnDestroy() => _selectedRegistry.SelectedBuildingChanged -= OnSelectedChanged;

        private void BuildFromCatalog()
        {
            Clear();

            BuildingCatalog catalog = _catalogProvider.TryGetCatalog();
            
            foreach (BuildingDefinition def in catalog.EnumerateAll())
            {
                Sprite icon = Resources.Load<Sprite>(def.SpriteResourcePath);
                
                if (icon == null)
                    continue;

                BuildingItemView view = Instantiate(_itemPrefab, _itemsRoot);
                view.Initialize(def.Id, icon, OnItemClicked);
                spawnedItems.Add(view);
            }
        }

        private void Clear()
        {
            foreach (BuildingItemView view in spawnedItems)
            {
                if (view != null)
                    Destroy(view.gameObject);
            }

            spawnedItems.Clear();
        }

        private void OnItemClicked(BuildingItemView view)
        {
            _selectedRegistry.Select(view.BuildingId);
        }

        private void OnSelectedChanged(string selectedId)
        {
            foreach (BuildingItemView item in spawnedItems)
                item.SetSelected(item.BuildingId == selectedId);
        }
    }
}