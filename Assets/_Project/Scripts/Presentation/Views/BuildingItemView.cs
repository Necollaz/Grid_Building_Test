using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Presentation.UI
{
    public class BuildingItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _selectedOverlay;
        [SerializeField] private Button _clickArea;

        private string _buildingId;

        public string BuildingId => _buildingId;

        public void Initialize(string buildingId, Sprite icon, System.Action<BuildingItemView> onClick)
        {
            _buildingId = buildingId;
            
            if (_icon != null)
                _icon.sprite = icon;

            if (_clickArea != null)
            {
                _clickArea.onClick.RemoveAllListeners();
                _clickArea.onClick.AddListener(() => onClick?.Invoke(this));
            }

            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (_selectedOverlay != null)
                _selectedOverlay.SetActive(selected);
        }
    }
}