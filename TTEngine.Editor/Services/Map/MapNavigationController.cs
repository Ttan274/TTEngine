using System.Windows.Input;

namespace TTEngine.Editor.Services.Map
{
    public class MapNavigationController
    {
        private readonly MapRenderer _renderer;
        private readonly Action _redraw;

        private float _moveSpeed = 400f;
        private float _zoomSpeed = 0.1f;

        public MapNavigationController(
            MapRenderer renderer,
            Action redraw)
        {
            _renderer = renderer;
            _redraw = redraw;
        }

        public void HandleKeyDown(Key key)
        {
            bool moved = true;

            switch (key)
            {
                case Key.W:
                    _renderer.MoveCamera(0, -_moveSpeed);
                    break;
                case Key.S:
                    _renderer.MoveCamera(0, _moveSpeed);
                    break;
                case Key.A:
                    _renderer.MoveCamera(-_moveSpeed, 0);
                    break;
                case Key.D:
                    _renderer.MoveCamera(_moveSpeed, 0);
                    break;
                default:
                    moved = false;
                    break;
            }

            if (moved)
                _redraw();
        }

        public void HandleMouseWheel(int delta)
        {
            if (delta > 0)
                _renderer.ChangeZoom(_zoomSpeed);
            else
                _renderer.ChangeZoom(-_zoomSpeed);

            _redraw();
        }
    }
}
