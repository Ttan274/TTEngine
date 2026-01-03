using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models;
using TTEngine.Editor.Services;
using System.Diagnostics;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Map
        private TileMapModel _tileMap = new();
        private bool _isPainting = false;
        private int _brushSize = 1;
        private Rectangle _hoverRect;

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
        private int[] ActiveTiles => _tileMap.Layers[editorState.ActiveLayer.LayerType];

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
            LayerEditor.DataContext = editorState;
            TileTools.DataContext = editorState;

            foreach (var layer in editorState.Layers)
            {
                layer.VisibilityChanged += OnLayerVisibilityChanged;
            }

            _entityDefinitions = EntityDefinitionService.Load();

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Undo,
                (_, _) => Undo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Redo,
                (_, _) => Redo()
            ));

            //Tile Tool Panel Events
            TileTools.ToolModeChanged += mode => _currentToolMode = mode;
            TileTools.BrushSizechanged += size => _brushSize = size;

            TileTools.SaveClicked += OnSaveRequested;
            TileTools.LoadClicked += OnLoadRequested;
            TileTools.StartGameClicked += OnStartRequested;

            _tileMap.Init();

            DrawGrid();
            CreateHoverRect();
        }

        #endregion

        #region Mouse Events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
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
            UpdateHover(e.GetPosition(MapCanvas));

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
            _hoverRect.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Button Events

        private void OnSaveRequested() => SaveMap(this, new RoutedEventArgs());

        private void OnLoadRequested() => LoadMap(this, new RoutedEventArgs());

        private void OnStartRequested() => StartGame(this, new RoutedEventArgs());

        private void OnLayerVisibilityChanged(EditorLayer layer)
            => DrawGrid();

        private void StartGame(object sender, RoutedEventArgs e) => Process.Start(EditorPaths.GetEngineExe());

        private void OpenDefinitiosns(object sender, RoutedEventArgs e) => Inspector.SetContent(new EntityDefinitionPanel(_entityDefinitions));
        
        private void OpenTileManager(object sender, RoutedEventArgs e) => Inspector.SetContent(new TileManagementPanel(editorState));

        #endregion

        #region Drawing
        //Canvas Methods
        private void DrawGrid()
        {
            //Mapcanvas settings
            MapCanvas.Children.Clear();
            MapCanvas.Width = _tileMap.Width * _tileMap.TileSize;
            MapCanvas.Height = _tileMap.Height * _tileMap.TileSize;

            //Grid overlay
            DrawGridOverlay();
            
            //Drawing Visible Layer Types
            foreach (var layer in editorState.Layers)
            {
                if (!layer.IsVisible)
                    continue;

                DrawLayer(layer.LayerType);
            }

            DrawPlayerSpawn();
            DrawEnemySpawns();

            if (_hoverRect != null && !MapCanvas.Children.Contains(_hoverRect))
            {
                MapCanvas.Children.Add(_hoverRect);
            }
        }

        private void DrawGridOverlay()
        {
            for (int y = 0; y < _tileMap.Height; y++)
            {
                for (int x = 0; x < _tileMap.Width; x++)
                {
                    Rectangle grid = new Rectangle
                    {
                        Width = _tileMap.TileSize,
                        Height = _tileMap.TileSize,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 0.5,
                        Fill = Brushes.DimGray,
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(grid, x * _tileMap.TileSize);
                    Canvas.SetTop(grid, y * _tileMap.TileSize);

                    MapCanvas.Children.Add(grid);
                }
            }
        }

        private void DrawLayer(MapLayerType layerType)
        {
            var tiles = _tileMap.Layers[layerType];

            for (int y = 0; y < _tileMap.Height; y++)
            {
                for (int x = 0; x < _tileMap.Width; x++)
                {
                    int index = _tileMap.GetIndex(x, y);
                    int tile = tiles[index];

                    if (tile == 0)
                        continue;

                    Rectangle rect = new Rectangle
                    {
                        Width = _tileMap.TileSize,
                        Height = _tileMap.TileSize,
                        Stroke = GetTileStroke(tile, layerType),
                        StrokeThickness = (layerType == MapLayerType.Collision) ? 2.0 : 0.0,
                        Fill = GetTileBrush(tile, layerType),
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(rect, x * _tileMap.TileSize);
                    Canvas.SetTop(rect, y * _tileMap.TileSize);
                    MapCanvas.Children.Add(rect);
                }
            }
        }

        private Brush GetTileBrush(int tile, MapLayerType layer)
        {
            var def = TileDefinitionService.GetById(tile);
            if (def == null)
                return Brushes.Transparent;

            if (layer == MapLayerType.Collision)
                return Brushes.Transparent;

            return def.CollisionType switch
            {
                CollisionType.Ground => Brushes.LightGreen,
                CollisionType.Wall => Brushes.SaddleBrown,
                _ => Brushes.Transparent
            };
        }

        private Brush GetTileStroke(int tile, MapLayerType layer)
        {
            if (layer != MapLayerType.Collision)
                return Brushes.Transparent;

            var def = TileDefinitionService.GetById(tile);
            if(def == null)
                return Brushes.Transparent;

            return def.CollisionType switch
            {
                CollisionType.Ground => Brushes.Orange,
                CollisionType.Wall => Brushes.Red,
                _ => Brushes.Transparent
            };
        }

        private void DrawPlayerSpawn()
        {
            if (_tileMap.PlayerSpawn == null)
                return;

            double cx = (_tileMap.PlayerSpawn.Position.X + 0.5) * _tileMap.TileSize;
            double cy = (_tileMap.PlayerSpawn.Position.Y + 0.5) * _tileMap.TileSize;

            Path star = new Path
            {
                Fill = Brushes.Gold,
                Stroke = Brushes.DarkOrange,
                StrokeThickness = 2,
                Data = Geometry.Parse(
                        "M 0,-10 L 3,-3 L 10,0 L 3,3 L 0,10 L -3,3 L -10,0 L -3,-3 Z"
                        ),
                IsHitTestVisible = false
            };

            Canvas.SetLeft(star, cx);
            Canvas.SetTop(star, cy);
            star.RenderTransform = new TranslateTransform(-10, -10);

            MapCanvas.Children.Add(star);
        }

        private void DrawEnemySpawns()
        {
            foreach (var spawn in _tileMap.EnemySpawns)
            {
                double cx = (spawn.Position.X + 0.5) * _tileMap.TileSize;
                double cy = (spawn.Position.Y + 0.5) * _tileMap.TileSize;

                Ellipse e = new Ellipse
                {
                    Width = _tileMap.TileSize * 0.5,
                    Height = _tileMap.TileSize * 0.5,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent,
                    IsHitTestVisible = false,
                    Tag = "EnemySpawn"
                };

                Canvas.SetLeft(e, cx - e.Width / 2);
                Canvas.SetTop(e, cy - e.Height / 2);

                MapCanvas.Children.Add(e);

            }
        }

        private void CreateHoverRect()
        {
            _hoverRect = new Rectangle
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                IsHitTestVisible = false
            };

            MapCanvas.Children.Add(_hoverRect);
        }

        private void UpdateHover(Point pos)
        {
            int x = (int)(pos.X / _tileMap.TileSize);
            int y = (int)(pos.Y / _tileMap.TileSize);

            if (x < 0 || y < 0 || x >= _tileMap.Width || y >= _tileMap.Height)
            {
                _hoverRect.Visibility = Visibility.Hidden;
                return;
            }

            _hoverRect.Visibility = Visibility.Visible;

            _hoverRect.Width = _brushSize * _tileMap.TileSize;
            _hoverRect.Height = _brushSize * _tileMap.TileSize;

            _hoverRect.Stroke = editorState.IsActiveLayerLocked ? Brushes.Gray : Brushes.Yellow;

            Canvas.SetLeft(_hoverRect, x * _tileMap.TileSize);
            Canvas.SetTop(_hoverRect, y * _tileMap.TileSize);
        }

        #endregion

        #region Painting
        private void PaintTile(Point pos) => ApplyBrush(pos, true);

        private void EraseTile(Point pos) => ApplyBrush(pos, false);

        private void ApplyBrush(Point pos, bool isPaint)
        {
            if (isPaint && editorState.SelectedTile == null)
                return;

            int baseX = (int)(pos.X / _tileMap.TileSize);
            int baseY = (int)(pos.Y / _tileMap.TileSize);

            for (int y = 0; y < _brushSize; y++)
            {
                for (int x = 0; x < _brushSize; x++)
                {
                    int tx = baseX + x;
                    int ty = baseY + y;

                    if (tx < 0 || ty < 0 || tx >= _tileMap.Width || ty >= _tileMap.Height)
                        continue;

                    int index = _tileMap.GetIndex(tx, ty);
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

            int startX = (int)(pos.X / _tileMap.TileSize);
            int startY = (int)(pos.Y / _tileMap.TileSize);

            if (startX < 0 || startY < 0 || startX >= _tileMap.Width || startY >= _tileMap.Height)
                return;

            int startIndex = _tileMap.GetIndex(startX, startY);
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

                if (cx < 0 || cy < 0 || cx >= _tileMap.Width || cy >= _tileMap.Height)
                    continue;

                int index = _tileMap.GetIndex(cx, cy);

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
            if (_tileMap.PlayerSpawn != null && _tileMap.PlayerSpawn.Position.X == x && _tileMap.PlayerSpawn.Position.Y == y)
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

            foreach (var spawn in _tileMap.EnemySpawns)
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
                    int index = _tileMap.GetIndex(_currentSelection.TileX, _currentSelection.TileY);
                    Inspector.SetContent(new TileSpawnInspector(_currentSelection.TileX, _currentSelection.TileY, ActiveTiles[index]));
                    break;
                case SelectionType.Player:
                    Inspector.SetContent(new PlayerSpawnInspector(_currentSelection.PlayerSpawnModel, _entityDefinitions));
                    break;
                case SelectionType.Enemy:
                    Inspector.SetContent(new EnemySpawnInspector(_currentSelection.EnemySpawnModel, _entityDefinitions));
                    break;
                default:
                    Inspector.Clear();
                    break;
            }
        }

        private bool TryGetTilePosition(Point pos, out int tileX, out int tileY)
        {
            tileX = (int)(pos.X / _tileMap.TileSize);
            tileY = (int)(pos.Y / _tileMap.TileSize);

            if (tileX < 0 || tileY < 0 ||
            tileX >= _tileMap.Width || tileY >= _tileMap.Height)
                return false;

            return true;
        }

        private bool HandlePlayerSpawn(Point pos)
        {
            if (!TryGetTilePosition(pos, out int x, out int y))
                return false;

            if(_tileMap.PlayerSpawn == null)
            {
                _tileMap.PlayerSpawn = new PlayerSpawnModel
                {
                    Position = new Point(x, y),
                };
            }

            _tileMap.PlayerSpawn.Position = new Point(x, y);
            DrawGrid();
            return true;
        }

        private bool HandleEnemySpawn(Point pos, MouseButtonEventArgs e)
        {
            if (!TryGetTilePosition(pos, out int x, out int y))
                return false;

            var existing = _tileMap.EnemySpawns
                                   .FirstOrDefault(s => s.Position.X == x && s.Position.Y == y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(existing == null)
                {
                    _tileMap.EnemySpawns.Add(new EnemySpawnModel
                    {
                        Position = new Point(x, y),
                        DefinitionId = _entityDefinitions.FirstOrDefault().Id     //need update
                    });
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if(existing != null)
                    _tileMap.EnemySpawns.Remove(existing);
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

        #region Save & Load
        
        private void SaveMap(object sender, RoutedEventArgs e)
        {
            var data = new TileMapData
            {
                Width = _tileMap.Width,
                Height = _tileMap.Height,
                TileSize = _tileMap.TileSize,
                Layers = _tileMap.Layers,
                PlayerSpawn = new SpawnDto
                {
                    X = _tileMap.PlayerSpawn.Position.X,
                    Y = _tileMap.PlayerSpawn.Position.Y,
                    DefinitionId = "Player"
                },
                EnemySpawns = _tileMap.EnemySpawns.Select(p => new SpawnDto
                {
                    X = p.Position.X,
                    Y = p.Position.Y,
                    DefinitionId = p.DefinitionId,
                }).ToList()
            };

            MapFileService.Save(data);
        }

        private void LoadMap(object sender, RoutedEventArgs e)
        {
            TileMapData data = MapFileService.Load();

            _tileMap.Width = data.Width;
            _tileMap.Height = data.Height;
            _tileMap.TileSize = data.TileSize;
            _tileMap.Layers = data.Layers;
            _tileMap.PlayerSpawn = new PlayerSpawnModel
            {
                Position = new Point(data.PlayerSpawn.X, data.PlayerSpawn.Y),
                DefinitionId = data.PlayerSpawn.DefinitionId
            };
            _tileMap.EnemySpawns.Clear();

            foreach (var sp in data.EnemySpawns)
            {
                _tileMap.EnemySpawns.Add(new EnemySpawnModel
                {
                    Position = new Point(sp.X, sp.Y),
                    DefinitionId = sp.DefinitionId,
                });
            }

            DrawGrid();
        }

        #endregion
    }
}