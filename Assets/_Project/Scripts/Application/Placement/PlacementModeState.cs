namespace ProjectGame.Scripts.Application.Placement
{
    public delegate void OnPlacementModeChange(PlacementInteractionModeType mode);
    
    public class PlacementModeState
    {
        public event OnPlacementModeChange PlacementModeChanged;
        
        public PlacementInteractionModeType CurrentMode { get; private set; } = PlacementInteractionModeType.None;
        
        public void SetMode(PlacementInteractionModeType mode)
        {
            if (CurrentMode == mode)
                return;

            CurrentMode = mode;
            
            PlacementModeChanged?.Invoke(CurrentMode);
        }
    }
}