using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ProjectGame.Scripts.Application.Persistence;

namespace ProjectGame.Scripts.Presentation.UI
{
    public class SaveLoadUiConnector : MonoBehaviour
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        [Inject] private SaveLoadCoordinator _coordinator;

        private void Awake()
        {
            _saveButton.onClick.AddListener(() => _coordinator.SaveCurrent());
            _loadButton.onClick.AddListener(() => _coordinator.LoadFromDisk());
        }
    }
}