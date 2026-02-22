using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Selection;

namespace TTEngine.Editor.EditorServices.Interaction
{
    public class SelectionController
    {
        private readonly EditorState _state;

        public SelectionController(EditorState state)
        {
            _state = state;
        }

        public void HandleSelection(int x, int y)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            if (CheckInteractables(scene, x, y))
                return;

            if (CheckEntities(scene, x, y))
                return;

            if(IsValid(x, y, scene.Map))
            {
                int tileValue = scene.Map.CollisionTiles[y][x];
                _state.CurrentSelection = new TileSelectionViewModel(x, y, tileValue);
            }
        }

        private bool CheckEntities(Scene scene, int x, int y)
        {
            var player = scene.Spawns.Player;

            if(player != null && player.X == x && player.Y == y)
            {
                var def = _state.Definition.EntityDefinitions.
                    FirstOrDefault(d => d.Id == "Player");

                if(def != null)
                    _state.CurrentSelection = new PlayerSelectionViewModel(x, y, def);

                return true;
            }

            var enemy = scene.Spawns.Enemies.FirstOrDefault(e => e.X == x && e.Y == y);

            if (enemy != null)
            {
                var def = _state.Definition.EntityDefinitions.FirstOrDefault(d => d.Id == enemy.DefinitionId);

                if (def != null)
                    _state.CurrentSelection = new EnemySelectionViewModel(x, y, def);

                return true;
            }

            return false;
        }

        private bool CheckInteractables(Scene scene, int x, int y)
        {
            var interactable = scene.Spawns.Interactables.FirstOrDefault(i => i.X == x && i.Y == y);

            if (interactable != null)
            {
                var def = _state.Definition.InteractableDefinitions.FirstOrDefault(d => d.Id == interactable.DefinitionId);

                if (def != null)
                    _state.CurrentSelection = new InteractableSelectionViewModel(x, y, def.Id, def.Type);

                return true;
            }

            //var trap = scene.Spawns.Traps.FirstOrDefault(t => t.X == x && t.Y == y);

            //if(trap != null)
            //{
            //    var def = _state.Definition.TrapDefinitions.FirstOrDefault(d => d.Id == trap.DefinitionId);

            //    if (def != null)
            //    {
            //        //Trap selection view model eklenicek
            //        //_state.CurrentSelection = new TrapSelectionViewModel(x, y, def.Id,);
            //    }
            //    return true;
            //}

            return false;
        }

        //Helper
        private bool IsValid(int x, int y, MapData map)
        {
            return x >= 0 && y >= 0 && x < map.Width && y < map.Height;
        }
    }
}
