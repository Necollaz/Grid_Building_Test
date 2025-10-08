using UnityEngine;
using Zenject;
using ProjectGame.Scripts.Application.Persistence;

namespace ProjectGame.Scripts.Presentation.UI
{
    public class JsonClipboardMenu : MonoBehaviour
    {
        [Inject] private SaveLoadCoordinator _coordinator;

        public void CopyJsonToClipboard()
        {
            string json = _coordinator.ExportJson();
            GUIUtility.systemCopyBuffer = json;
        }

        public void PasteJsonFromClipboard()
        {
            string json = GUIUtility.systemCopyBuffer;
            
            if (string.IsNullOrWhiteSpace(json))
                return;

            _coordinator.ImportJsonAndRebuild(json);
        }
    }
}