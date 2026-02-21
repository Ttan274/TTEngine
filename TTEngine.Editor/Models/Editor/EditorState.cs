using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorState : ObservableObject
    {
        //Selection
        private object _currentSelection;
        public object CurrentSelection
        {
            get => _currentSelection;
            set => SetProperty(ref _currentSelection, value);
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
            //LevelService = level;
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
    }
}
