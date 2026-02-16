using System;
using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Panels;

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
            var map = _state.ActiveMap;
            if (map == null)
                return;

            if (CheckInteractables(map, x, y))
                return;

            if (CheckEntities(map, x, y))
                return;

            // Default Tile
            int index = map.GetIndex(x, y);
            int tileValue = map.Layers[_state.ActiveLayer.LayerType][index];

            _state.CurrentSelection = new TileSelectionViewModel(x, y, tileValue);
        }

        private bool CheckEntities(TileMapModel map, int x, int y)
        {
            // Player
            if (map.PlayerSpawn != null &&
                map.PlayerSpawn.Position.X == x &&
                map.PlayerSpawn.Position.Y == y)
            {
                var def = _state.EntityDefinitions.FirstOrDefault(d => d.Id == map.PlayerSpawn.DefinitionId);

                if(def != null)
                    _state.CurrentSelection = new PlayerSelectionViewModel(x, y, def);

                return true;
            }

            // Enemy
            var enemy = map.EnemySpawns
                .FirstOrDefault(e => e.Position.X == x && e.Position.Y == y);

            if (enemy != null)
            {
                var def = _state.EntityDefinitions.FirstOrDefault(d => d.Id == enemy.DefinitionId);

                if (def != null)
                    _state.CurrentSelection = new EnemySelectionViewModel(x, y, def);

                return true;
            }

            return false;
        }

        private bool CheckInteractables(TileMapModel map, int x, int y)
        {
            var interactable = map.Interactables.FirstOrDefault(i => i.X == x && i.Y == y);

            if (interactable != null)
            {
                var def = _state.InteractableDefinitions.FirstOrDefault(d => d.Id == interactable.DefinitionId);

                if (def != null)
                {
                    //Bunun modeli eklenmedi
                }

                return true;
            }

            //trap eklenicek

            return false;
        }
    }
}
