using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Panels;

namespace TTEngine.Editor.EditorServices.Interaction
{
    public class SelectionController
    {
        private readonly EditorState _state;
        private readonly Action<UserControl> _showInspector;

        public SelectionController(EditorState state, Action<UserControl> showInspector)
        {
            _state = state;
            _showInspector = showInspector;
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

            _showInspector(
                new TileSpawnInspector(x, y, tileValue));
        }

        private bool CheckEntities(TileMapModel map, int x, int y)
        {
            // Player
            if (map.PlayerSpawn != null &&
                map.PlayerSpawn.Position.X == x &&
                map.PlayerSpawn.Position.Y == y)
            {
                _showInspector(
                    new PlayerSpawnInspector(
                        map.PlayerSpawn,
                        _state.EntityDefinitions.ToList()));

                return true;
            }

            // Enemy
            var enemy = map.EnemySpawns
                .FirstOrDefault(e => e.Position.X == x && e.Position.Y == y);

            if (enemy != null)
            {
                _showInspector(
                    new EnemySpawnInspector(
                        enemy,
                        _state.EntityDefinitions.ToList()));

                return true;
            }

            return false;
        }

        private bool CheckInteractables(TileMapModel map, int x, int y)
        {
            var interactable = map.Interactables.FirstOrDefault(i => i.X == x && i.Y == y);

            if (interactable != null)
            {
                _showInspector(
                    new InteractableInspector(
                        interactable,
                        _state.InteractableDefinitions));

                return true;
            }

            //trap eklenicek

            return false;
        }
    }
}
