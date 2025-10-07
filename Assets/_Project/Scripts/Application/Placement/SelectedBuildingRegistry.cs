namespace _Project.Scripts.Application.Placement
{
    public delegate void OnSelectedBuildingChange(string buildingId);
    
    public class SelectedBuildingRegistry
    {
        public event OnSelectedBuildingChange SelectedBuildingChanged;

        public string SelectedBuildingId { get; private set; }

        public void Select(string buildingId)
        {
            if (SelectedBuildingId == buildingId)
                return;

            SelectedBuildingId = buildingId;
            
            SelectedBuildingChanged?.Invoke(SelectedBuildingId);
        }

        public void Clear()
        {
            if (SelectedBuildingId == null)
                return;

            SelectedBuildingId = null;
            
            SelectedBuildingChanged?.Invoke(SelectedBuildingId);
        }
    }
}