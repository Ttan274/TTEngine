using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Selection;

namespace TTEngine.Editor.Services.Map
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

            if (CheckSceneObjects(scene, x, y))
                return;

            if(IsValid(x, y, scene.Map))
            {
                int tileValue = scene.Map.CollisionTiles[y][x];
                _state.CurrentSelection = new TileSelectionViewModel(x, y, tileValue);
            }
        }

        private bool CheckSceneObjects(Scene scene, int x, int y)
        {
            var obj = scene.SceneObjects.FirstOrDefault(o => o.X == x && o.Y == y);

            if (obj == null)
                return false;

            _state.CurrentSelection = new SceneObjectSelectionViewModel(obj);
            _state.SceneSession.SelectedObject = obj;

            return true;
        }

        //Helper
        private bool IsValid(int x, int y, MapData map)
        {
            return x >= 0 && y >= 0 && x < map.Width && y < map.Height;
        }
    }
}
