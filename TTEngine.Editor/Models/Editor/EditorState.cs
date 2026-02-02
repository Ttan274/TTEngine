using System.Collections.ObjectModel;
using System.ComponentModel;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor
{
    public enum PlacementMode
    {
        Tile,
        Interactable
    }

    public class EditorState : INotifyPropertyChanged
    {
        public EditorConsole Console { get; } = new EditorConsole();

        public ObservableCollection<EditorLayer> Layers { get; } =
            new ObservableCollection<EditorLayer>
            {
                new EditorLayer(MapLayerType.Background),
                new EditorLayer(MapLayerType.Collision) {IsActive = true},
                new EditorLayer(MapLayerType.Decoration),
                new EditorLayer(MapLayerType.Interactable)
            };

        public EditorLayer ActiveLayer =>
            Layers.First(l => l.IsActive);

        public ObservableCollection<TileDefinition> TileDefinitions { get; }
        public ObservableCollection<InteractableDefinition> InteractableDefinitions { get; }
        
        //Placement
        public PlacementMode _activePlacementMode;
        public PlacementMode ActivePlacementMode
        {
            get => _activePlacementMode;
            set
            {
                _activePlacementMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivePlacementMode)));
            }
        }

        private TileDefinition _selectedTile;
        public TileDefinition SelectedTile
        {
            get => _selectedTile;
            set
            {
                _selectedTile = value;
                
                if(value != null)
                {
                    _selectedInteractable = null;
                    _activePlacementMode = PlacementMode.Tile;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedInteractable)));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTile)));
            }
        }

        private InteractableDefinition _selectedInteractable;
        public InteractableDefinition SelectedInteractable
        {
            get => _selectedInteractable;
            set
            {
                _selectedInteractable = value;

                if (value != null)
                {
                    _selectedTile = null;
                    _activePlacementMode = PlacementMode.Interactable;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTile)));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedInteractable)));
            }
        }

        //Active Layer & Active Map
        public bool IsActiveLayerLocked =>
            ActiveLayer != null && ActiveLayer.IsLocked;

        private TileMapModel _activeMap;
        public TileMapModel ActiveMap
        {
            get => _activeMap;
            set
            {
                _activeMap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveMap)));
            }
        }

        private string _activeMapId;
        public string ActiveMapId
        {
            get => _activeMapId;
            set
            {
                _activeMapId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveMapId)));
            }
        }

        public const string DEFAULT_MAP_ID = "Map_Default";
        public bool IsDefaultMap => ActiveMapId == DEFAULT_MAP_ID;

        public EditorState()
        {
            TileDefinitions = new ObservableCollection<TileDefinition>(TileDefinitionService.Load());
            InteractableDefinitions = new ObservableCollection<InteractableDefinition>(InteractableFileService.Load());
        }

        public void SetActiveLayer(EditorLayer layer)
        {
            foreach (var l in Layers)
                l.IsActive = false;

            layer.IsActive = true;
        }

        public void SaveActiveMap()
        {
            if (IsDefaultMap)
                return;

            if (ActiveMap == null || string.IsNullOrEmpty(ActiveMapId))
                return;

            var dto = MapFileService.ToDto(ActiveMap);
            MapFileService.Save(ActiveMapId, dto);

            Console.Log($"{ActiveMapId} saved");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
