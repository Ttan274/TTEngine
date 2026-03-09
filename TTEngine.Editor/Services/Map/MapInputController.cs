using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Services.Map
{
    public class MapInputController
    {
        private readonly EditorState _state;
        private readonly MapRenderer _renderer;
        private readonly MapInteractionController _interaction;
        private readonly SelectionController _selection;
        private readonly MapNavigationController _navigation;
        private readonly Func<int> _brushSize;

        public MapInputController(EditorState state, 
            MapRenderer renderer,
            MapInteractionController interaction,
            SelectionController selection,
            MapNavigationController navigation,
            Func<int> brushSize)
        {
            _state = state;
            _renderer = renderer;
            _interaction = interaction;
            _selection = selection;
            _navigation = navigation;
            _brushSize = brushSize;
        }

        public void MouseDown(Canvas canvas, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(canvas);

            //Selection override
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (TryGetTilePosition(pos, out int sx, out int sy))
                    _selection.HandleSelection(sx, sy);
                return;
            }

            Mouse.Capture(canvas);

            //Selection
            if (TryGetTilePosition(pos, out int x, out int y))
                _selection.HandleSelection(x, y);

            //Interaction
            _interaction.OnMouseDown(pos, e);
        }

        public void MouseMove(Canvas canvas, MouseEventArgs e)
        {
            var pos = e.GetPosition(canvas);

            _renderer.UpdateHover(pos, _brushSize());
            _interaction.OnMouseMove(pos, e);
        }

        public void MouseUp()
        {
            Mouse.Capture(null);
            _interaction.OnMouseUp();
        }

        public void MouseWheel(int delta)
        {
            _navigation.HandleMouseWheel(delta);
        }

        public void KeyDown(Canvas canvas, Key key)
        {
            //if (!canvas.IsMouseOver)
            //    return;

            if (Keyboard.FocusedElement is TextBox)
                return;

            if (Keyboard.FocusedElement is ComboBox)
                return;

            _navigation.HandleKeyDown(key);
        }

        private bool TryGetTilePosition(Point pos, out int x, out int y)
        {
            x = 0;
            y = 0;

            if (_state.SceneSession.ActiveScene == null)
                return false;

            var map = _state.SceneSession.ActiveScene.Map;

            x = (int)(pos.X / map.TileSize);
            y = (int)(pos.Y / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return false;

            return true;
        }
    }
}
