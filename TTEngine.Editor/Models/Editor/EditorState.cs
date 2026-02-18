using System.ComponentModel;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorState : INotifyPropertyChanged
    {
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

        //Services
        public LevelService LevelService { get; }
        public AnimationService AnimationService { get; }

        //States
        public ToolState Tool { get; }
        public PlacementState Placement { get; }
        public LayerState Layer { get; }
        public DefinitionCatalog Definition { get; }
        public MapSessionState MapSession { get; }

        //Console 
        public EditorConsole Console { get; }

        public EditorState(
            ToolState tool, 
            PlacementState placement, 
            LayerState layer,
            MapSessionState mapSession,
            DefinitionCatalog definition,
            EditorConsole console,
            LevelService level,
            AnimationService anim)
        {
            //State bindings
            Tool = tool;
            Placement = placement;
            Layer = layer;
            MapSession = mapSession;
            Definition = definition;

            //Console
            Console = console;

            //Services
            LevelService = level;
            AnimationService = anim;
            AnimationService.LoadAll();
        }

        //Map Session Helper
        public void SaveActiveMap()
        {
            MapSession.Save();
            Console.Log($"{MapSession.ActiveMapId} saved");
        }

        //Definition Helper
        public TileDefinition? GetTileById(int id)
           => Definition.TileDefinitions.FirstOrDefault(d => d.Id == id);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
