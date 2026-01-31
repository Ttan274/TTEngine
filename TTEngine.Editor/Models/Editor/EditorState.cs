using System.Collections.ObjectModel;
using System.ComponentModel;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorState
    {
        public EditorConsole Console { get; } = new EditorConsole();

        public ObservableCollection<EditorLayer> Layers { get; } =
            new ObservableCollection<EditorLayer>
            {
                new EditorLayer(MapLayerType.Background),
                new EditorLayer(MapLayerType.Collision) {IsActive = true},
                new EditorLayer(MapLayerType.Decoration)
            };

        public EditorLayer ActiveLayer =>
            Layers.First(l => l.IsActive);

        public ObservableCollection<TileDefinition> TileDefinitions { get; }

        private TileDefinition _selectedTile;
        public TileDefinition SelectedTile
        {
            get => _selectedTile;
            set
            {
                _selectedTile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTile)));
            }
        }

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

        public EditorState()
        {
            TileDefinitions = new ObservableCollection<TileDefinition>(TileDefinitionService.Load());
        }

        public void SetActiveLayer(EditorLayer layer)
        {
            foreach (var l in Layers)
                l.IsActive = false;

            layer.IsActive = true;
        }

        public TileDefinition GetSelectedTile()
            => TileDefinitions.FirstOrDefault(t => t.Id == SelectedTile.Id);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
