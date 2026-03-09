using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Services.Map
{
    public class MapInteractionController
    {
        private readonly EditorState _state;
        private readonly MapRenderer _renderer;
        private readonly Action _redraw;

        private readonly Stack<TileBatchCommand> _undoStack = new();
        private readonly Stack<TileBatchCommand> _redoStack = new();

        private TileBatchCommand _currentBatch;
        private bool _isPainting;
        private int _brushSize = 1;

        public MapInteractionController(EditorState state, MapRenderer renderer, Action redraw)
        {
            _state = state;
            _renderer = renderer;
            _redraw = redraw;
        }

        #region Mouse
        public void OnMouseDown(Point pos, MouseButtonEventArgs e)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            if(_state.Placement.ActivePlacementMode == PlacementMode.Tile)
            {
                _isPainting = true;
                _currentBatch = new TileBatchCommand();
                switch (_state.Tool.CurrentToolMode)
                {
                    case ToolMode.Brush:
                        HandleBrush(pos, e);
                        break;
                    case ToolMode.Fill:
                        if (e.LeftButton == MouseButtonState.Pressed)
                            HandleFill(pos);
                        break;
                }
            }
            else if(_state.Placement.ActivePlacementMode == PlacementMode.Object)
            {
                HandleObjectPlacement(pos, e);
            }
           
            _redraw();
        }

        public void OnMouseMove(Point pos, MouseEventArgs e)
        {
            if (!_isPainting)
                return;

            if (_state.Tool.CurrentToolMode != ToolMode.Brush)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
                ApplyBrush(pos, true);

            if (e.RightButton == MouseButtonState.Pressed)
                ApplyBrush(pos, false);

            _redraw();
        }

        public void OnMouseUp()
        {
            _isPainting = false;

            if(_currentBatch != null && !_currentBatch.IsEmpty())
            {
                _undoStack.Push(_currentBatch);
                _redoStack.Clear();
            }

            _currentBatch = null;
        }

        #endregion

        #region Brush

        public void SetBrushSize(int size) => _brushSize = size;

        private void HandleBrush(Point pos, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ApplyBrush(pos, true);

            if (e.RightButton == MouseButtonState.Pressed)
                ApplyBrush(pos, false);
        }

        private void ApplyBrush(Point pos, bool isPaint)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;
            var map = scene.Map;

            Point basePos = GetTilePos(pos, map);

            for (int y = 0; y < _brushSize; y++)
            {
                for (int x = 0; x < _brushSize; x++)
                {
                    int tx = (int)basePos.X + x;
                    int ty = (int)basePos.Y + y;

                    if (!IsValid(tx, ty, map))
                        continue;

                    int newValue = isPaint ? _state.Placement.SelectedTile?.Id ?? 0 : 0;
                    int oldValue = map.CollisionTiles[ty][tx];

                    if (oldValue == newValue)
                        continue;

                    map.CollisionTiles[ty][tx] = newValue;

                    _currentBatch?.Add(new TileChangeCommand(tx, ty, oldValue, newValue));
                }
            }
        }

        #endregion

        #region Fill

        private void HandleFill(Point pos)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;
            var map = scene.Map;

            if (_state.Placement.SelectedTile == null)
                return;

            Point basePos = GetTilePos(pos, map);
            int x = (int)basePos.X;
            int y = (int)basePos.Y;

            if (!IsValid(x, y, map))
                return;

            int targetValue = map.CollisionTiles[y][x];
            int newValue = _state.Placement.SelectedTile.Id;

            if (targetValue == newValue)
                return;

            Fill(x, y, targetValue, newValue, map);
        }

        private void Fill(int x, int y, int target, int newValue, MapData map)
        {
            Stack<(int x, int y)> stack = new();
            stack.Push((x, y));

            while (stack.Count > 0)
            {
                var (cx, cy) = stack.Pop();

                if (!IsValid(cx, cy, map))
                    continue;

                if (map.CollisionTiles[cy][cx] != target)
                    continue;

                int oldValue = map.CollisionTiles[cy][cx];
                map.CollisionTiles[cy][cx] = newValue;

                _currentBatch?.Add(new TileChangeCommand(cx, cy, oldValue, newValue));

                stack.Push((cx + 1, cy));
                stack.Push((cx - 1, cy));
                stack.Push((cx, cy + 1));
                stack.Push((cx, cy - 1));
            }
        }

        #endregion

        #region Object 

        private void HandleObjectPlacement(Point pos, MouseButtonEventArgs e)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var map = scene.Map;

            Point basePos = GetTilePos(pos, map);
            int x = (int)basePos.X;
            int y = (int)basePos.Y;

            if (!IsValid(x, y, map))
                return;

            var prefab = scene.SceneObjects.FirstOrDefault(o => o.X == x && o.Y == y);

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (_state.Placement.SelectedPrefab.Id == null)
                    return;

                //if (!EditorValidator.CanPlaceObject(scene, x, y))
                //    return;

                if(prefab == null)
                {
                    scene.SceneObjects.Add(new SceneObjectData
                    {
                        PrefabId = _state.Placement.SelectedPrefab.Id,
                        InstanceId = Guid.NewGuid().ToString(),
                        X = x,
                        Y = y,
                        IsActive = true
                    });
                }
            }
            else if(e.RightButton == MouseButtonState.Pressed)
            {
                if(prefab != null)
                    scene.SceneObjects.Remove(prefab);
            }

        }

        #endregion

        #region Undo/Redo

        public void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var batch = _undoStack.Pop();
            batch.Undo(scene.Map);

            _redoStack.Push(batch);
            _redraw();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var batch = _redoStack.Pop();
            batch.Redo(scene.Map);

            _undoStack.Push(batch);
            _redraw();
        }

        #endregion

        #region Helpers
        private Point GetTilePos(Point pos, MapData map)
        {
            //Screen to world pos
            Point world = new Point(
                    pos.X / _renderer.Zoom + _renderer.CameraX,
                    pos.Y / _renderer.Zoom + _renderer.CameraY
            );

            int x = (int)(world.X / map.TileSize);
            int y = (int)(world.Y / map.TileSize);

            return new Point(x, y);
        }

        private bool IsValid(int x, int y, MapData map)
        {
            return x >= 0 && y >= 0 && x < map.Width && y < map.Height;
        }
        #endregion
    }
}
