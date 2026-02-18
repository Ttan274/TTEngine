using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Models.Validation;

namespace TTEngine.Editor.EditorServices.Interaction
{
    public class MapInteractionController
    {
        private readonly EditorState _state;
        private readonly Action _redraw;

        private readonly Stack<TileBatchCommand> _undoStack = new();
        private readonly Stack<TileBatchCommand> _redoStack = new();

        private TileBatchCommand _currentBatch;
        private bool _isPainting;
        private int _brushSize = 1;

        public MapInteractionController(EditorState state, Action redraw)
        {
            _state = state;
            _redraw = redraw;
        }

        #region Mouse
        public void OnMouseDown(Point pos, MouseButtonEventArgs e)
        {
            if (_state.MapSession.ActiveMap == null || _state.Layer.IsActiveLayerLocked)
                return;

            _isPainting = true;
            _currentBatch = new TileBatchCommand();

            switch (_state.Tool.CurrentToolMode)
            {
                case ToolMode.Brush:
                    HandleBrush(pos, e);
                    HandlePlacementModes(pos, e);
                    break;
                case ToolMode.Fill:
                    if (e.LeftButton == MouseButtonState.Pressed)
                        HandleFill(pos);
                    break;
                case ToolMode.PlayerSpawn:
                    HandlePlayerSpawn(pos);
                    break;
                case ToolMode.EnemySpawn:
                    HandleEnemySpawn(pos, e);
                    break;
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
            var map = _state.MapSession.ActiveMap;

            if (!IsInLayer(MapLayerType.Collision))
                return;

            int baseX = (int)(pos.X / map.TileSize);
            int baseY = (int)(pos.Y / map.TileSize);

            for (int y = 0; y < _brushSize; y++)
            {
                for (int x = 0; x < _brushSize; x++)
                {
                    int tx = baseX + x;
                    int ty = baseY + y;

                    if (!IsValid(tx, ty))
                        continue;

                    int index = map.GetIndex(tx, ty);
                    int newValue = isPaint ? _state.Placement.SelectedTile?.Id ?? 0 : 0;

                    ApplyTileChange(index, newValue);
                }
            }
        }

        #endregion

        #region Fill

        private void HandleFill(Point pos)
        {
            var map = _state.MapSession.ActiveMap;

            if (!IsInLayer(MapLayerType.Collision))
                return;

            if (_state.Placement.SelectedTile == null)
                return;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (!IsValid(x, y))
                return;

            int[] tiles = map.Layers[_state.Layer.ActiveLayer.LayerType];

            int startIndex = map.GetIndex(x, y);
            int targetValue = tiles[startIndex];
            int newValue = _state.Placement.SelectedTile.Id;

            if (targetValue == newValue)
                return;

            Fill(x, y, targetValue, newValue, tiles);
        }

        private void Fill(int x, int y, int target, int newValue, int[] tiles)
        {
            var map = _state.MapSession.ActiveMap;

            Stack<(int x, int y)> stack = new();
            stack.Push((x, y));

            while (stack.Count > 0)
            {
                var (cx, cy) = stack.Pop();

                if (!IsValid(x, y))
                    return;

                int index = map.GetIndex(cx, cy);

                if (tiles[index] != target)
                    continue;

                ApplyTileChange(index, newValue);

                stack.Push((cx + 1, cy));
                stack.Push((cx - 1, cy));
                stack.Push((cx, cy + 1));
                stack.Push((cx, cy - 1));
            }
        }

        #endregion

        #region Entity Spawns

        private void HandlePlayerSpawn(Point pos)
        {
            var map = _state.MapSession.ActiveMap;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (!IsValid(x, y))
                return;

            if (!EditorValidator.CanPlacePlayer(map, x, y))
                return;

            if (map.PlayerSpawn == null)
                map.PlayerSpawn = new PlayerSpawnModel();

            map.PlayerSpawn.Position = new Point(x, y);
        }

        private void HandleEnemySpawn(Point pos, MouseButtonEventArgs e)
        {
            var map = _state.MapSession.ActiveMap;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (!IsValid(x, y))
                return;

            var existing = map.EnemySpawns
                .FirstOrDefault(s => s.Position.X == x && s.Position.Y == y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!EditorValidator.CanPlaceObject(map, x, y))
                    return;

                if (existing == null && _state.Definition.EntityDefinitions.Any())
                {
                    map.EnemySpawns.Add(new EnemySpawnModel
                    {
                        Position = new Point(x, y),
                        DefinitionId = _state.Definition.EntityDefinitions.First().Id
                    });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (existing != null)
                    map.EnemySpawns.Remove(existing);
            }
        }

        #endregion

        #region Interactables

        private void HandlePlacementModes(Point pos, MouseButtonEventArgs e)
        {
            if (_state.Placement.ActivePlacementMode == PlacementMode.Interactable)
                HandleInteractablePlacement(pos, e);

            if (_state.Placement.ActivePlacementMode == PlacementMode.Trap)
                HandleTrapPlacement(pos, e);
        }

        private void HandleInteractablePlacement(Point pos, MouseButtonEventArgs e)
        {
            var map = _state.MapSession.ActiveMap;

            if (!IsInLayer(MapLayerType.Interactable))
                return;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (!IsValid(x, y))
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_state.Placement.SelectedInteractable == null)
                    return;

                if (!EditorValidator.CanPlaceObject(map, x, y))
                    return;

                if (!map.Interactables.Any(i => i.X == x && i.Y == y))
                {
                    map.Interactables.Add(new InteractableModel
                    {
                        X = x,
                        Y = y,
                        DefinitionId = _state.Placement.SelectedInteractable.Id
                    });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                var existing = map.Interactables
                    .FirstOrDefault(i => i.X == x && i.Y == y);

                if (existing != null)
                    map.Interactables.Remove(existing);
            }
        }

        private void HandleTrapPlacement(Point pos, MouseButtonEventArgs e)
        {
            var map = _state.MapSession.ActiveMap;

            if (!IsInLayer(MapLayerType.Interactable))
                return;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (!IsValid(x, y))
                return;
                
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_state.Placement.SelectedTrap == null)
                    return;

                if (!EditorValidator.CanPlaceObject(map, x, y))
                    return;

                if (!map.Traps.Any(t => t.X == x && t.Y == y))
                {
                    map.Traps.Add(new TrapModel
                    {
                        X = x,
                        Y = y,
                        DefinitionId = _state.Placement.SelectedTrap.Id
                    });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                var existing = map.Traps
                    .FirstOrDefault(t => t.X == x && t.Y == y);

                if (existing != null)
                    map.Traps.Remove(existing);
            }
        }

        #endregion

        #region Undo/Redo

        public void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var batch = _undoStack.Pop();
            batch.Undo(_state.MapSession.ActiveMap.Layers[_state.Layer.ActiveLayer.LayerType]);
            _redoStack.Push(batch);
            _redraw();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var batch = _redoStack.Pop();
            batch.Redo(_state.MapSession.ActiveMap.Layers[_state.Layer.ActiveLayer.LayerType]);
            _undoStack.Push(batch);
            _redraw();
        }

        #endregion

        #region Helpers

        private void ApplyTileChange(int index, int newValue)
        {
            var tiles = _state.MapSession.ActiveMap.Layers[_state.Layer.ActiveLayer.LayerType];
            int oldValue = tiles[index];

            if (oldValue == newValue)
                return;

            var command = new TileChangeCommand(index, oldValue, newValue);
            command.Redo(tiles);
            _currentBatch?.Add(command);
        }

        private bool IsValid(int x, int y)
        {
            var map = _state.MapSession.ActiveMap;
            return x >= 0 && y >= 0 && x < map.Width && y < map.Height;
        }

        private bool IsInLayer(MapLayerType type) => _state.Layer.ActiveLayer.LayerType == type;
       
        #endregion
    }
}
