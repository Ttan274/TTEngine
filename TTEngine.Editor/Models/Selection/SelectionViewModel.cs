using TTEngine.Editor.Models.Scene;

namespace TTEngine.Editor.Models.Selection
{
    public abstract class SelectionViewModel
    {
        public int X { get; }
        public int Y { get; }

        protected SelectionViewModel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class TileSelectionViewModel : SelectionViewModel
    {
        public int TileId { get; }

        public TileSelectionViewModel(int x, int y, int tileId)
            : base(x, y)
        {
            TileId = tileId;
        }
    }

    public class SceneObjectSelectionViewModel : SelectionViewModel
    {
        public SceneObjectData SceneObject { get; }

        public string PrefabId => SceneObject.PrefabId;

        public SceneObjectSelectionViewModel(SceneObjectData obj) 
            : base(obj.X, obj.Y)
        {
            SceneObject = obj;
        }
    }
}
