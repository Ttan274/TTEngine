using System.Collections.ObjectModel;
using System.ComponentModel;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor
{
    public enum PlacementMode
    {
        Tile,
        Interactable,
        Trap
    }

    public class EditorState : INotifyPropertyChanged
    {
        public EditorConsole Console { get; } = new EditorConsole();

        //Layers -- Silinecek
        //public ObservableCollection<EditorLayer> Layers { get; } =
        //    new ObservableCollection<EditorLayer>
        //    {
        //        new EditorLayer(MapLayerType.Background),
        //        new EditorLayer(MapLayerType.Collision) {IsActive = true},
        //        new EditorLayer(MapLayerType.Decoration),
        //        new EditorLayer(MapLayerType.Interactable)
        //    };

        //public EditorLayer ActiveLayer =>
        //    Layers.First(l => l.IsActive);

        //Definitions
        public ObservableCollection<TileDefinition> TileDefinitions { get; }
        public ObservableCollection<InteractableDefinition> InteractableDefinitions { get; }
        public ObservableCollection<TrapDefinition> TrapDefinitions { get; }
        public ObservableCollection<EntityDefinitionModel> EntityDefinitions { get; }

        //Selection
        private SelectionViewModel _currentSelection;
        public SelectionViewModel CurrentSelection
        {
            get => _currentSelection;
            set
            {
                _currentSelection = value;
                OnPropertyChanged(nameof(CurrentSelection));
            }
        }

        //Active Layer & Active Map
        
        //silinecek
        //public bool IsActiveLayerLocked =>
        //    ActiveLayer != null && ActiveLayer.IsLocked;

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

        //Services
        public MapService MapService { get; }
        public LevelService LevelService { get; }
        public AnimationService AnimationService { get; }

        //Repositories
        public JsonRepository<EntityDefinitionModel> EntityRepository { get; }
        public JsonRepository<TileDefinition> TileRepository { get; }
        public JsonRepository<InteractableDefinition> InteractableRepository { get; }
        public JsonRepository<TrapDefinition> TrapRepository { get; }

        //States
        public ToolState Tool { get; }
        public PlacementState Placement { get; }
        public LayerState Layer { get; }

        public EditorState(ToolState tool, PlacementState placement, LayerState layer)
        {
            //State binding
            Tool = tool;
            Placement = placement;
            Layer = layer;

            //Services
            MapService = new MapService();
            LevelService = new LevelService();
            AnimationService = new AnimationService(EditorPaths.Animation);
            AnimationService.LoadAll();

            //Entity Repo + Load Definitions
            EntityRepository = new JsonRepository<EntityDefinitionModel>(EditorPaths.EntityDefs);
            EntityDefinitions = new ObservableCollection<EntityDefinitionModel>(EntityRepository.GetAll());

            //Tile Repo + Load Definitions
            TileRepository = new JsonRepository<TileDefinition>(EditorPaths.TileDefs);
            TileDefinitions = new ObservableCollection<TileDefinition>(TileRepository.GetAll());

            //Interactable Repo + Load Definitions
            InteractableRepository = new JsonRepository<InteractableDefinition>(EditorPaths.InteractableDefs);
            InteractableDefinitions = new ObservableCollection<InteractableDefinition>(InteractableRepository.GetAll());

            //Trap Repo + Load Definitions
            TrapRepository = new JsonRepository<TrapDefinition>(EditorPaths.TrapDefs);
            TrapDefinitions = new ObservableCollection<TrapDefinition>(TrapRepository.GetAll()); 
        }

        //silinecek
        //public void SetActiveLayer(EditorLayer layer)
        //{
        //    foreach (var l in Layers)
        //        l.IsActive = false;

        //    layer.IsActive = true;
        //}

        public void SaveActiveMap()
        {
            if (IsDefaultMap)
                return;

            if (ActiveMap == null || string.IsNullOrEmpty(ActiveMapId))
                return;

            MapService.Save(ActiveMapId, ActiveMap);
            Console.Log($"{ActiveMapId} saved");
        }

        public TileDefinition? GetTileById(int id)
           => TileDefinitions.FirstOrDefault(d => d.Id == id);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
