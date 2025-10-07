using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using _Project.Scripts.Application.Persistence;

namespace _Project.Scripts.Presentation.UI
{
    public class SaveLoadUiConnector : MonoBehaviour
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _exportButton;
        [SerializeField] private Button _importButton;
        [SerializeField] private TMP_InputField _jsonField;

        [Inject] private SaveLoadCoordinator _coordinator;

        private void Awake()
        {
            _saveButton.onClick.AddListener(() => _coordinator.SaveCurrent());
            _loadButton.onClick.AddListener(() => _coordinator.LoadFromDisk());
            _exportButton.onClick.AddListener(() => _jsonField.text = _coordinator.ExportJson());
            _importButton.onClick.AddListener(() => _coordinator.ImportJsonAndRebuild(_jsonField.text));
        }
    }
}