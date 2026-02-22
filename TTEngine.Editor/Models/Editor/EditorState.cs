using TTEngine.Editor.Models.Editor.EditorStates;
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
        public AnimationService AnimationService { get; }

        //States
        public ToolState Tool { get; }
        public PlacementState Placement { get; }
        public DefinitionCatalog Definition { get; }
        public SceneSessionState SceneSession { get; }

        //Console 
        public EditorConsole Console { get; }

        public EditorState(
            ToolState tool, 
            PlacementState placement, 
            SceneSessionState sceneSession,
            DefinitionCatalog definition,
            EditorConsole console,
            AnimationService anim)
        {
            //State bindings
            Tool = tool;
            Placement = placement;
            SceneSession = sceneSession;
            Definition = definition;

            //Console
            Console = console;

            //Services
            AnimationService = anim;
            AnimationService.LoadAll();
        }

        //Map Session Helper
        public void SaveActiveMap()
        {
            SceneSession.Save();
            Console.Log("Active scene saved");
        }

        //Definition Helper
        public TileDefinition? GetTileById(int id)
           => Definition.TileDefinitions.FirstOrDefault(d => d.Id == id);
    }
}
