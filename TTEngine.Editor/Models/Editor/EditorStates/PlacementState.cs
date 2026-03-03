using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public enum PlacementMode
    {
        Tile,
        Object
    }

    public class PlacementState : ObservableObject
    {
        private PlacementMode _activePlacementMode;
        public PlacementMode ActivePlacementMode
        {
            get => _activePlacementMode;
            set => SetProperty(ref _activePlacementMode, value);
        }

        private TileDefinition _selectedTile;
        public TileDefinition SelectedTile
        {
            get => _selectedTile;
            set
            {
                if(SetProperty(ref _selectedTile, value) && value != null)
                {
                    SelectedPrefab = null;
                    ActivePlacementMode = PlacementMode.Tile;
                }
            }
        }

        private GameObject.GameObject _selectedPrefab;
        public GameObject.GameObject SelectedPrefab
        {
            get => _selectedPrefab;
            set
            {
                if(SetProperty(ref _selectedPrefab, value) && value != null)
                {
                    SelectedTile = null;
                    ActivePlacementMode = PlacementMode.Object;
                }
            }
        }

        public void ClearSelection()
        {
            SelectedTile = null;
            SelectedPrefab = null;
        }
    }
}
