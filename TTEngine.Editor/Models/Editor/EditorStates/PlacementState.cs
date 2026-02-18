using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
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
                    SelectedInteractable = null;
                    SelectedTrap = null;
                    ActivePlacementMode = PlacementMode.Tile;
                }
            }
        }

        private InteractableDefinition _selectedInteractable;
        public InteractableDefinition SelectedInteractable
        {
            get => _selectedInteractable;
            set
            {
                if (SetProperty(ref _selectedInteractable, value) && value != null)
                {
                    SelectedTile = null;
                    SelectedTrap = null;
                    ActivePlacementMode = PlacementMode.Interactable;
                }
            }
        }

        private TrapDefinition _selectedTrap;
        public TrapDefinition SelectedTrap
        {
            get => _selectedTrap;
            set
            {
                if (SetProperty(ref _selectedTrap, value) && value != null)
                {
                    SelectedTile = null;
                    SelectedInteractable = null;
                    ActivePlacementMode = PlacementMode.Trap;
                }
            }
        }

        public void ClearSelection()
        {
            SelectedTile = null;
            SelectedInteractable = null;
            SelectedTrap = null;
        }
    }
}
