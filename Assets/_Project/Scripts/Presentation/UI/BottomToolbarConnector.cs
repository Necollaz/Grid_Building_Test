using UnityEngine;
using Zenject;
using _Project.Scripts.Application.Placement;

namespace _Project.Scripts.Presentation.UI
{
    public class BottomToolbarConnector : MonoBehaviour
    {
        [SerializeField] private BottomToolbarView _view;

        [Inject] private PlacementModeState _modeState;

        private void Awake()
        {
            _view.PlaceButton.onClick.AddListener(OnPlaceClicked);
            _view.DeleteButton.onClick.AddListener(OnDeleteClicked);
            
            _modeState.PlacementModeChanged += OnModeChanged;
            
            OnModeChanged(_modeState.CurrentMode);
        }

        private void OnDestroy()
        {
            _view.PlaceButton.onClick.RemoveListener(OnPlaceClicked);
            _view.DeleteButton.onClick.RemoveListener(OnDeleteClicked);
            
            _modeState.PlacementModeChanged -= OnModeChanged;
        }

        private void OnPlaceClicked() => _modeState.SetMode(PlacementInteractionModeType.Place);
        private void OnDeleteClicked() => _modeState.SetMode(PlacementInteractionModeType.Delete);

        private void OnModeChanged(PlacementInteractionModeType mode)
        {
            bool isPlace = mode == PlacementInteractionModeType.Place;
            bool isDelete = mode == PlacementInteractionModeType.Delete;
            
            _view.ShowPlaceActive(isPlace);
            _view.ShowDeleteActive(isDelete);
        }
    }
}