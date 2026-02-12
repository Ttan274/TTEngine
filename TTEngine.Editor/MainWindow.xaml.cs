using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTEngine.Editor.EditorServices.Rendering;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Models.Validation;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Map
        private TileMapModel ActiveMap => editorState.ActiveMap;
        private MapRenderer _renderer;
        private bool _isPainting = false;
        private int _brushSize = 1;
        private Rectangle _hoverRect;
        public const string DEFAULT_MAP_ID = "Map_Default";

        //Stack
        private Stack<TileBatchCommand> _undoStack = new();
        private Stack<TileBatchCommand> _redoStack = new();
        private TileBatchCommand _currentBatch;

        //ToolPanel
        private ToolMode _currentToolMode = ToolMode.Brush;

        //Selection + Entity Def
        private SelectionModel _currentSelection = new();
        private List<EntityDefinitionModel> _entityDefinitions;

        //Layer
        public EditorState editorState { get; } = new EditorState();

        //Active Tiles
        private int[] ActiveTiles => ActiveMap?.Layers[editorState.ActiveLayer.LayerType];

        public MainWindow()
        {
            InitializeComponent();
            WindowSetup();
        }

        #region Setup

        //Setup
        private void WindowSetup()
        {
            //Context setup
            AnimationDefinitionService.LoadAll();
            LayerEditor.DataContext = editorState;
            TileTools.DataContext = editorState;
            ConsoleEditor.DataContext = editorState;
            ToolHost.BindEditor(editorState);

            editorState.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(editorState.ActiveMap))
                    DrawGrid();
            };

            foreach (var layer in editorState.Layers)
                layer.VisibilityChanged += OnLayerVisibilityChanged;

            _entityDefinitions = EntityDefinitionService.Load();

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Undo,
                (_, _) => Undo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Redo,
                (_, _) => Redo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Save,
                (_, _) => editorState.SaveActiveMap()
            ));

            //Tile Tool Panel Events
            TileTools.ToolModeChanged += mode => _currentToolMode = mode;
            TileTools.BrushSizechanged += size => _brushSize = size;
            TileTools.StartGameClicked += OnStartRequested;

            _renderer = new MapRenderer(MapCanvas, editorState);
            EnsureDefaultMap();
            DrawGrid();
        }

        #endregion

        #region Mouse Events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editorState.IsDefaultMap)
                return;

            if (editorState.IsActiveLayerLocked)
                return;

            _isPainting = true;
            Mouse.Capture(MapCanvas);

            Point pos = e.GetPosition(MapCanvas);

            if (TryGetTilePosition(pos, out int sx, out int sy))
                HandleSelection(sx, sy);

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                _isPainting = false;
                Mouse.Capture(null);
                return;
            }

            if (editorState.ActivePlacementMode == PlacementMode.Interactable)
            {
                HandleInteractablePlacement(pos, e);
                return;
            }

            if (editorState.ActivePlacementMode == PlacementMode.Trap)
            {
                HandleTrapPlacement(pos, e);
                return;
            }

            bool handled = _currentToolMode switch
            {
                ToolMode.PlayerSpawn => HandlePlayerSpawn(pos),
                ToolMode.EnemySpawn => HandleEnemySpawn(pos, e),
                ToolMode.Fill => e.LeftButton == MouseButtonState.Pressed && HandleFill(pos),
                _ => false
            };

            if (!handled)
                HandleBrush(pos, e);

            if (_currentToolMode != ToolMode.Brush)
            {
                Mouse.Capture(null);
                _isPainting = false;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (editorState.IsDefaultMap)
                return;

            _renderer.UpdateHover(e.GetPosition(MapCanvas), _brushSize);

            if (editorState.IsActiveLayerLocked)
                return;

            if (_currentToolMode == ToolMode.Fill) return;

            if (!_isPainting) return;

            if (e.LeftButton == MouseButtonState.Pressed)
                PaintTile(e.GetPosition(MapCanvas));
            if (e.RightButton == MouseButtonState.Pressed)
                EraseTile(e.GetPosition(MapCanvas));
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPainting = false;
            Mouse.Capture(null);

            if (_currentBatch != null && !_currentBatch.IsEmpty())
            {
                _undoStack.Push(_currentBatch);
                _redoStack.Clear();
            }

            _currentBatch = null;
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _renderer.MakeHoverUnvisible();
        }

        #endregion

        #region Button Events

        private void OnStartRequested()
        {
            var validation = ValidateMap();

            if (!validation.IsValid)
                return;

            StartGame(this, new RoutedEventArgs());
        }

        private void OnLayerVisibilityChanged(EditorLayer layer)
            => DrawGrid();

        private void StartGame(object sender, RoutedEventArgs e) => Process.Start(EditorPaths.GetEngineExe());

        #endregion

        #region Drawing
        //Canvas Methods
        private void DrawGrid() => _renderer.Draw();

        #endregion

        #region Painting
        private void PaintTile(Point pos) => ApplyBrush(pos, true);

        private void EraseTile(Point pos) => ApplyBrush(pos, false);

        private void ApplyBrush(Point pos, bool isPaint)
        {
            if(TryGetTilePosition(pos, out int xPos, out int yPos))
            {
                if (HasTrapAt(xPos, yPos) || HasInteractableAt(xPos, yPos))
                    return;
            }

            if (editorState.ActivePlacementMode != PlacementMode.Tile)
                return;

            if (isPaint && editorState.SelectedTile == null)
                return;

            int baseX = (int)(pos.X / ActiveMap.TileSize);
            int baseY = (int)(pos.Y / ActiveMap.TileSize);

            for (int y = 0; y < _brushSize; y++)
            {
                for (int x = 0; x < _brushSize; x++)
                {
                    int tx = baseX + x;
                    int ty = baseY + y;

                    if (tx < 0 || ty < 0 || tx >= ActiveMap.Width || ty >= ActiveMap.Height)
                        continue;

                    int index = ActiveMap.GetIndex(tx, ty);
                    int newValue = isPaint ? editorState.SelectedTile.Id : 0;

                    ApplyTileChange(index, newValue);
                }
            }

            DrawGrid();
        }

        private void Fill(Point pos)
        {
            if (editorState.SelectedTile == null)
                return;

            int startX = (int)(pos.X / ActiveMap.TileSize);
            int startY = (int)(pos.Y / ActiveMap.TileSize);

            if (startX < 0 || startY < 0 || startX >= ActiveMap.Width || startY >= ActiveMap.Height)
                return;

            int startIndex = ActiveMap.GetIndex(startX, startY);
            int targetValue = ActiveTiles[startIndex];
            int newValue = editorState.SelectedTile.Id;

            if (targetValue == newValue)
                return;

            FillFlood(startX, startY, targetValue, newValue);

            if (_currentBatch != null && !_currentBatch.IsEmpty())
            {
                _undoStack.Push(_currentBatch);
                _redoStack.Clear();
            }

            _currentBatch = null;
            DrawGrid();
        }

        private void FillFlood(int x, int y, int target, int newValue)
        {
            Stack<(int x, int y)> stack = new();
            stack.Push((x, y));

            while (stack.Count > 0)
            {
                var (cx, cy) = stack.Pop();

                if (cx < 0 || cy < 0 || cx >= ActiveMap.Width || cy >= ActiveMap.Height)
                    continue;

                int index = ActiveMap.GetIndex(cx, cy);

                if (ActiveTiles[index] != target)
                    continue;

                ApplyTileChange(index, newValue);

                stack.Push((cx + 1, cy));
                stack.Push((cx - 1, cy));
                stack.Push((cx, cy + 1));
                stack.Push((cx, cy - 1));
            }
        }

        private void ApplyTileChange(int index, int newValue)
        {
            int oldValue = ActiveTiles[index];
            if (oldValue == newValue)
                return;

            var command = new TileChangeCommand(index, oldValue, newValue);
            command.Redo(ActiveTiles);
            _currentBatch?.Add(command);
        }
        #endregion

        #region Mouse Event Helpers
        //Mouse Event Helpers
        private void HandleSelection(int x, int y)
        {
            if(editorState.ActiveLayer.LayerType == MapLayerType.Interactable)
            {
                foreach (var interactable in ActiveMap.Interactables)
                {
                    if (interactable.X == x && interactable.Y == y)
                    {
                        _currentSelection = new SelectionModel
                        {
                            Type = SelectionType.Interactable,
                            InteractableModel = interactable
                        };
                    }
                    ShowInspector();
                    return;
                }
            }

            if (ActiveMap.PlayerSpawn != null && ActiveMap.PlayerSpawn.Position.X == x && ActiveMap.PlayerSpawn.Position.Y == y)
            {
                _currentSelection = new SelectionModel
                {
                    Type = SelectionType.Player,
                    PlayerSpawnModel = new PlayerSpawnModel
                    {
                        Position = new Point(x, y)
                    }
                };

                ShowInspector();
                return;
            }

            foreach (var spawn in ActiveMap.EnemySpawns)
            {
                if (spawn.Position.X == x && spawn.Position.Y == y)
                {
                    _currentSelection = new SelectionModel
                    {
                        Type = SelectionType.Enemy,
                        EnemySpawnModel = new EnemySpawnModel
                        {
                            Position = spawn.Position,
                            DefinitionId = spawn.DefinitionId
                        }
                    };

                    ShowInspector();
                    return;
                }
            }


            _currentSelection = new SelectionModel
            {
                Type = SelectionType.Tile,
                TileX = x,
                TileY = y
            };

            ShowInspector();
        }

        private void ShowInspector()
        {
            switch (_currentSelection.Type)
            {
                case SelectionType.Tile:
                    int index = ActiveMap.GetIndex(_currentSelection.TileX, _currentSelection.TileY);
                    Inspector.SetContent(new TileSpawnInspector(_currentSelection.TileX, _currentSelection.TileY, ActiveTiles[index]));
                    break;
                case SelectionType.Player:
                    Inspector.SetContent(new PlayerSpawnInspector(_currentSelection.PlayerSpawnModel, _entityDefinitions));
                    break;
                case SelectionType.Enemy:
                    Inspector.SetContent(new EnemySpawnInspector(_currentSelection.EnemySpawnModel, _entityDefinitions));
                    break;
                case SelectionType.Interactable:
                    Inspector.SetContent(new InteractableInspector(_currentSelection.InteractableModel, editorState.InteractableDefinitions));
                    break;
                default:
                    Inspector.Clear();
                    break;
            }
        }

        private bool TryGetTilePosition(Point pos, out int tileX, out int tileY)
        {
            tileX = (int)(pos.X / ActiveMap.TileSize);
            tileY = (int)(pos.Y / ActiveMap.TileSize);

            if (tileX < 0 || tileY < 0 ||
            tileX >= ActiveMap.Width || tileY >= ActiveMap.Height)
                return false;

            return true;
        }

        private bool HandlePlayerSpawn(Point pos)
        {
            if (editorState.IsDefaultMap)
                return false;

            if (!TryGetTilePosition(pos, out int x, out int y))
                return false;

            if(ActiveMap.PlayerSpawn == null)
            {
                ActiveMap.PlayerSpawn = new PlayerSpawnModel
                {
                    Position = new Point(x, y),
                };
            }

            ActiveMap.PlayerSpawn.Position = new Point(x, y);
            DrawGrid();
            return true;
        }

        private bool HandleEnemySpawn(Point pos, MouseButtonEventArgs e)
        {
            if (editorState.IsDefaultMap)
                return false;

            if (!TryGetTilePosition(pos, out int x, out int y))
                return false;

            var existing = ActiveMap.EnemySpawns
                                   .FirstOrDefault(s => s.Position.X == x && s.Position.Y == y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(existing == null)
                {
                    ActiveMap.EnemySpawns.Add(new EnemySpawnModel
                    {
                        Position = new Point(x, y),
                        DefinitionId = _entityDefinitions.FirstOrDefault().Id     //need update
                    });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if(existing != null)
                    ActiveMap.EnemySpawns.Remove(existing);
            }

            DrawGrid();
            return true;
        }

        private bool HandleFill(Point pos)
        {
            _currentBatch = new TileBatchCommand();
            Fill(pos);
            return true;
        }

        private void HandleBrush(Point pos, MouseButtonEventArgs e)
        {
            _currentBatch = new TileBatchCommand();
            if (e.RightButton == MouseButtonState.Pressed)
                EraseTile(e.GetPosition(MapCanvas));
            else if (e.LeftButton == MouseButtonState.Pressed)
                PaintTile(e.GetPosition(MapCanvas));
        }

        private void HandleInteractablePlacement(Point pos, MouseButtonEventArgs e)
        {
            if (editorState.ActivePlacementMode != PlacementMode.Interactable || editorState.ActiveLayer.LayerType != MapLayerType.Interactable)
                return;

            if (editorState.SelectedInteractable == null)
                return;

            if (!TryGetTilePosition(pos, out int x, out int y))
                return;

            if (HasTrapAt(x, y))
                return;

            int index = ActiveMap.GetIndex(x, y);

            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (HasInteractableAt(x, y))
                    return;

                ActiveMap.Layers[MapLayerType.Interactable][index] = 1;
                ActiveMap.Interactables.Add(new InteractableModel
                {
                    X = x,
                    Y = y,
                    DefinitionId = editorState.SelectedInteractable.Id
                });

            }
            else if(e.LeftButton == MouseButtonState.Released)
            {
                var existing = ActiveMap.Interactables.FirstOrDefault(i => i.X == x && i.Y == y);
                if (existing != null)
                {
                    ActiveMap.Interactables.Remove(existing);
                    ActiveMap.Layers[MapLayerType.Interactable][index] = 0;

                }
            }

            DrawGrid();
        }

        private void HandleTrapPlacement(Point pos, MouseButtonEventArgs e)
        {
            if (editorState.ActivePlacementMode != PlacementMode.Trap || editorState.ActiveLayer.LayerType != MapLayerType.Interactable)
                return;

            if (editorState.SelectedTrap == null)
                return;

            if (!TryGetTilePosition(pos, out int x, out int y))
                return;

            if (HasInteractableAt(x, y))
                return;

            int index = ActiveMap.GetIndex(x, y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (HasTrapAt(x, y))
                    return;

                ActiveMap.Layers[MapLayerType.Interactable][index] = 1;
                ActiveMap.Traps.Add(new TrapModel
                {
                    X = x,
                    Y = y,
                    DefinitionId = editorState.SelectedTrap.Id
                });
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                var existing = ActiveMap.Traps.FirstOrDefault(i => i.X == x && i.Y == y);
                if (existing != null)
                {
                    ActiveMap.Layers[MapLayerType.Interactable][index] = 0;
                    ActiveMap.Traps.Remove(existing);
                }
            }

            DrawGrid();
        }

        #endregion

        #region Stack Methods
        //Stack Methods
        private void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var batch = _undoStack.Pop();
            batch.Undo(ActiveTiles);
            _redoStack.Push(batch);

            DrawGrid();
        }

        private void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var batch = _redoStack.Pop();
            batch.Redo(ActiveTiles);
            _undoStack.Push(batch);

            DrawGrid();
        }

        #endregion

        #region Load Map

        private void EnsureDefaultMap()
        {
            if(!MapFileService.Exists(DEFAULT_MAP_ID))
            {
                var model = new TileMapModel();
                model.Init();

                MapFileService.Save(DEFAULT_MAP_ID, MapFileService.ToDto(model));
            }

            var data = MapFileService.Load(DEFAULT_MAP_ID);
            var mapModel = MapFileService.FromDto(data);

            editorState.ActiveMapId = DEFAULT_MAP_ID;
            editorState.ActiveMap = mapModel;
            editorState.Console.Log($"{DEFAULT_MAP_ID} is loaded.");
        }

        #endregion

        #region Validation

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();
            EditorValidationResult result = new EditorValidationResult();

            //If player object not spawned
            if(ActiveMap.PlayerSpawn == null)
            {
                result.Errors.Add("Error1");
                editorState.Console.Log("Player spawn is missing");
                return result;
            }

            //If player object spawned on solid tile
            if(IsOnCollision(ActiveMap.PlayerSpawn.Position))
            {
                result.Errors.Add("Error-2");
                editorState.Console.Log("Player spawn is placed on a solid tile");
            }

            //If any enemy object spawned on solid tile
            foreach (var enemy in ActiveMap.EnemySpawns)
            {
                if(IsOnCollision(enemy.Position))
                {
                    result.Errors.Add("Error3");
                    editorState.Console.Log("Enemy spawn is placed on a solid tile");
                }
            }

            return result;
        }

        private bool IsOnCollision(Point position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;

            int index = ActiveMap.GetIndex(x, y);
            int tileId = ActiveMap.Layers[MapLayerType.Collision][index];

            if (tileId == 0)
                return false;

            var def = TileDefinitionService.GetById(tileId);
            if (def == null)
                return false;

            return def.CollisionType == CollisionType.Wall;
        }

        #endregion
   
        private bool HasInteractableAt(int x, int y)
        {
            return ActiveMap.Interactables.Any(i => i.X == x && i.Y == y);
        }

        private bool HasTrapAt(int x, int y)
        {
            return ActiveMap.Traps.Any(i => i.X == x && i.Y == y);
        }
    }
}