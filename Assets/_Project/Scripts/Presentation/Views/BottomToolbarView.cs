using UnityEngine;
using UnityEngine.UI;

namespace ProjectGame.Scripts.Presentation.UI
{
    public class BottomToolbarView : MonoBehaviour
    {
        [SerializeField] private Button _placeButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private GameObject _placeActiveOverlay;
        [SerializeField] private GameObject _deleteActiveOverlay;

        public Button PlaceButton => _placeButton;
        public Button DeleteButton => _deleteButton;

        public void ShowPlaceActive(bool active)
        {
            if (_placeActiveOverlay != null)
                _placeActiveOverlay.SetActive(active);
        }

        public void ShowDeleteActive(bool active)
        {
            if (_deleteActiveOverlay != null)
                _deleteActiveOverlay.SetActive(active);
        }
    }
}