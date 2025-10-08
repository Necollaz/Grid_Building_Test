using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using ProjectGame.Scripts.Application.Placement;
using ProjectGame.Scripts.Domain.Buildings;
using ProjectGame.Scripts.Domain.Grid;
using ProjectGame.Scripts.Presentation.Views;
using ProjectGame.Scripts.Infrastructure.Config;
using ProjectGame.Scripts.Infrastructure.Input;

namespace ProjectGame.Scripts.Presentation.UI
{
    public class PlacementController : MonoBehaviour
    {
        private const int ROTATE_ANGLE = 360;
        
        [SerializeField] private PlacementCursorView _cursorView;
        [SerializeField] private RectTransform _uiPanelRoot;
        
        private Camera _camera;
        private Vector2Int _currentGrid;
        private int _rotationDegrees;
        private bool _pressStartedOverUI;
        private bool _confirmPrevFrame;
        
        [Inject] private PlacementModeState _modeState;
        [Inject] private SelectedBuildingRegistry _selectedRegistry;
        [Inject] private JsonBuildingCatalogProvider _catalogProvider;
        [Inject] private PlacementApplicationService _placementService;
        [Inject] private DeletionApplicationService _deletionService;
        [Inject] private GridCoordinateConverter _converter;
        [Inject] private GridOccupancyMap _occupancy;
        [Inject] private PlacementInputOrchestrator _input;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _modeState.PlacementModeChanged += OnModeChanged;
            _selectedRegistry.SelectedBuildingChanged += OnSelectedChanged;
            
            ApplyInitialCursorState();
        }

        private void OnDisable()
        {
            _modeState.PlacementModeChanged -= OnModeChanged;
            _selectedRegistry.SelectedBuildingChanged -= OnSelectedChanged;
        }

        private void Update()
        {
            bool confirmNow = _input.ConfirmPressed;
            
            if (confirmNow && !_confirmPrevFrame)
                _pressStartedOverUI = IsPointerOverUI() || HasUiFocusInPanel();
            
            if (!confirmNow && _confirmPrevFrame)
                _pressStartedOverUI = false;
            
            _confirmPrevFrame = confirmNow;
            
            if (!IsPointerOverUI())
                UpdateGridByMouse();
            
            UpdateGridByKeyboard();
            UpdateRotation();
            UpdateCursor();

            if (_modeState.CurrentMode == PlacementInteractionModeType.Place)
                TryConfirmPlacement();

            if (_modeState.CurrentMode == PlacementInteractionModeType.Delete)
                TryConfirmDeletion();

            _input.ResetFrameDeltas();
        }

        private (Vector2Int position, Vector2Int orientedSize) ResolveFootprintForInstance(BuildingView view)
        {
            BuildingCatalog catalog = _catalogProvider.TryGetCatalog();
            catalog.TryGet(view.BuildingId, out BuildingDefinition def);
            
            Vector2Int size = def != null ? def.CalculateFootprint(view.RotationDegrees) : Vector2Int.one;
            
            return (view.GridPosition, size);
            
        }
        private int NormalizeRotation(int value)
        {
            int rotation = value % ROTATE_ANGLE;
            
            if (rotation < 0)
                rotation += ROTATE_ANGLE;
            
            return rotation;
        }
        
        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;
            
            if (EventSystem.current.IsPointerOverGameObject())
                return true;
            
            if (_uiPanelRoot != null &&
                RectTransformUtility.RectangleContainsScreenPoint(_uiPanelRoot, _input.MouseScreenPosition, _camera))
                return true;

            return false;
        }
        
        private bool HasUiFocusInPanel()
        {
            if (EventSystem.current == null || _uiPanelRoot == null)
                return false;
            
            GameObject focused = EventSystem.current.currentSelectedGameObject;
            
            return focused != null && focused.transform.IsChildOf(_uiPanelRoot.transform);
        }
        
        private void UpdateGridByMouse()
        {
            Ray ray = _camera.ScreenPointToRay(_input.MouseScreenPosition);
            Plane plane = new Plane(Vector3.forward, 0f);
            
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 world = ray.GetPoint(enter);
                _currentGrid = _converter.WorldToGrid(world);
            }
        }

        private void UpdateGridByKeyboard()
        {
            if (_input.PendingGridDelta != Vector2Int.zero)
                _currentGrid += _input.PendingGridDelta;
        }

        private void UpdateRotation()
        {
            if (_input.RotationDeltaDegrees != 0)
                _rotationDegrees = NormalizeRotation(_rotationDegrees + _input.RotationDeltaDegrees);
        }

        private void UpdateCursor()
        {
            bool isPlace = _modeState.CurrentMode == PlacementInteractionModeType.Place;
            bool hasSelection = !string.IsNullOrEmpty(_selectedRegistry.SelectedBuildingId);
            _cursorView.TrySetVisible(isPlace && hasSelection);
            
            if (!(isPlace && hasSelection))
                return;

            BuildingCatalog catalog = _catalogProvider.TryGetCatalog();
            
            if (!catalog.TryGet(_selectedRegistry.SelectedBuildingId, out BuildingDefinition def))
                return;

            Sprite sprite = Resources.Load<Sprite>(def.SpriteResourcePath);
            
            if (sprite != null)
            {
                _cursorView.TrySetCellWorldSize(_converter.CellSizeUnits);
                _cursorView.TrySetSprite(sprite);

                Vector2Int footprint = def.CalculateFootprint(_rotationDegrees);
                _cursorView.TrySetFootprintCells(footprint);
                _cursorView.TrySetRotation(_rotationDegrees);
                _cursorView.TrySetGridOrigin(_currentGrid, _converter.GridToWorld);

                bool valid = _occupancy.IsAreaFreeVerbose(_currentGrid, footprint);
                _cursorView.TrySetValidity(valid);
            }
        }

        private void TryConfirmPlacement()
        {
            if (!_input.ConfirmPressed)
                return;
            
            if (_pressStartedOverUI || IsPointerOverUI())
                return;
            
            if (string.IsNullOrEmpty(_selectedRegistry.SelectedBuildingId))
                return;

            BuildingCatalog catalog = _catalogProvider.TryGetCatalog();
            
            if (!catalog.TryGet(_selectedRegistry.SelectedBuildingId, out BuildingDefinition def))
                return;

            Sprite sprite = Resources.Load<Sprite>(def.SpriteResourcePath);
            
            if (sprite == null)
                return;
            
            _placementService.TryPlace(def, sprite, _currentGrid, _rotationDegrees, out _, out _);
        }

        private void TryConfirmDeletion()
        {
            if (!_input.ConfirmPressed)
                return;

            if (_pressStartedOverUI || IsPointerOverUI())
                return;
            
            _deletionService.TryDeleteAtCell(_currentGrid, ResolveFootprintForInstance);
        }

        private void ApplyInitialCursorState()
        {
            bool visible = _modeState.CurrentMode == PlacementInteractionModeType.Place && !string.
                IsNullOrEmpty(_selectedRegistry.SelectedBuildingId);
            
            _cursorView.TrySetVisible(visible);
        }
        
        private void OnModeChanged(PlacementInteractionModeType mode)
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
            
            ApplyInitialCursorState();
        }
        private void OnSelectedChanged(string id) => ApplyInitialCursorState();
    }
}