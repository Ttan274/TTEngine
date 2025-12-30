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

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TileMapModel _tileMap = new();
        private bool _isPainting = false;
        private int _brushSize = 1;
        private Rectangle _hoverRect;
        private Stack<TileBatchCommand> _undoStack = new();
        private Stack<TileBatchCommand> _redoStack = new();
        private TileBatchCommand _currentBatch;
        private ToolMode _currentToolMode = ToolMode.Brush;
        private TileType _currentTileType = TileType.Ground;
        private SelectionModel _currentSelection = new();
        private const string LIVE_MAP_PATH = @"..\..\..\..\Maps\active_map.json";
        private const string ENGINE_PATH = @"..\..\..\..\x64\Debug\TTEngine.exe";

        public MainWindow()
        {
            InitializeComponent();
            WindowSetup();
            _tileMap.Init();
            DrawGrid();
            CreateHoverRect();
        }

        #region Mouse Events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPainting = true;
            Mouse.Capture(MapCanvas);

            Point pos = e.GetPosition(MapCanvas);

            if (TryGetTilePosition(pos, out int sx, out int sy))
                HandleSelection(sx, sy);

            if(Keyboard.IsKeyDown(Key.LeftAlt))
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

            if(_currentToolMode != ToolMode.Brush)
            {
                Mouse.Capture(null);
                _isPainting = false;
            }
        }

        private void HandleSelection(int x, int y)
        {
            if (_tileMap.PlayerSpawn.X == x && _tileMap.PlayerSpawn.Y == y)
            {
                _currentSelection = new SelectionModel
                {
                    Type = SelectionType.Player,
                    Spawn = new Point(x, y)
                };

                ShowInspector();
                return;
            }

            foreach (var spawn in _tileMap.EnemySpawns)
            {
                if (spawn.X == x && spawn.Y == y)
                {
                    _currentSelection = new SelectionModel
                    {
                        Type = SelectionType.Enemy,
                        Spawn = new Point(spawn.X, spawn.Y)
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
                    Inspector.SetContent(new TileSpawnInspector(_currentSelection.TileX, _currentSelection.TileY, _tileMap.Tiles[index]));
                    break;
                case SelectionType.Player:
                    Inspector.SetContent(new PlayerSpawnInspector(_currentSelection.Spawn));
                    break;
                case SelectionType.Enemy:
                    Inspector.SetContent(new EnemySpawnInspector(_currentSelection.Spawn));
                    break;
                default:
                    Inspector.Clear();
                    break;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateHover(e.GetPosition(MapCanvas));

            if (_currentToolMode == ToolMode.Fill) return;

            if (!_isPainting) return;

            if(e.LeftButton == MouseButtonState.Pressed)
                PaintTile(e.GetPosition(MapCanvas));
            if (e.RightButton == MouseButtonState.Pressed)
                EraseTile(e.GetPosition(MapCanvas));
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPainting = false;
            Mouse.Capture(null);

            if(_currentBatch != null && !_currentBatch.IsEmpty())
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

        private void SaveMap(object sender, RoutedEventArgs e)
        {
            var data = new TileMapData
            {
                Width = _tileMap.Width,
                Height = _tileMap.Height,
                TileSize = _tileMap.TileSize,
                Tiles = _tileMap.Tiles,
                PlayerSpawnX = _tileMap.PlayerSpawn.X,
                PlayerSpawnY = _tileMap.PlayerSpawn.Y,
                EnemySpawns = _tileMap.EnemySpawns.Select(p => new SpawnDto { X = p.X, Y = p.Y }).ToList()
            };

            MapFileService.Save(LIVE_MAP_PATH, data);
        }

        private void LoadMap(object sender, RoutedEventArgs e)
        {
            TileMapData data = MapFileService.Load(LIVE_MAP_PATH);

            _tileMap.Width = data.Width;
            _tileMap.Height = data.Height;
            _tileMap.TileSize = data.TileSize;
            _tileMap.Tiles = data.Tiles;
            _tileMap.PlayerSpawn = new Point(data.PlayerSpawnX, data.PlayerSpawnY);
            _tileMap.EnemySpawns.Clear();

            foreach (var sp in data.EnemySpawns)
                _tileMap.EnemySpawns.Add(new Point(sp.X, sp.Y));

            DrawGrid();
        }

        private void StartGame(object sender, RoutedEventArgs e) => Process.Start(ENGINE_PATH, LIVE_MAP_PATH);

        #endregion

        #region Methods

        //Setup
        private void WindowSetup()
        {
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Undo,
                (_, _) => Undo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Redo,
                (_, _) => Redo()
            ));

            TileTools.ToolModeChanged += mode => _currentToolMode = mode;
            TileTools.TileTypeChanged += type => _currentTileType = type;
            TileTools.BrushSizechanged += size => _brushSize = size;

            TileTools.SaveClicked += OnSaveRequested;
            TileTools.LoadClicked += OnLoadRequested;
            TileTools.StartGameClicked += OnStartRequested;
        }

        //Canvas Methods
        private void DrawGrid()
        {
            //Mapcanvas settings
            MapCanvas.Children.Clear();
            MapCanvas.Width = _tileMap.Width * _tileMap.TileSize;
            MapCanvas.Height = _tileMap.Height * _tileMap.TileSize;
            MapCanvas.Background = Brushes.Transparent;

            for (int y = 0; y < _tileMap.Height; y++)
            {
                for (int x = 0; x < _tileMap.Width; x++)
                {
                    int index = _tileMap.GetIndex(x, y);
                    int tile = _tileMap.Tiles[index];

                    Rectangle rect = new Rectangle
                    {
                        Width = _tileMap.TileSize,
                        Height = _tileMap.TileSize,
                        Stroke = Brushes.Gray,
                        Fill = tile switch
                        {
                            (int)TileType.None => Brushes.DimGray,
                            (int)TileType.Ground => Brushes.LightGreen,
                            (int)TileType.Wall => Brushes.DarkSlateGray,
                            _ => Brushes.Magenta
                        },
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(rect, x * _tileMap.TileSize);
                    Canvas.SetTop(rect, y * _tileMap.TileSize);

                    MapCanvas.Children.Add(rect);
                }
            }

            if (_hoverRect != null && !MapCanvas.Children.Contains(_hoverRect))
            {
                MapCanvas.Children.Add(_hoverRect);
            }

            DrawPlayerSpawn();
            DrawEnemySpawns();
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

            Canvas.SetLeft(_hoverRect, x * _tileMap.TileSize);
            Canvas.SetTop(_hoverRect, y * _tileMap.TileSize);
        }

        private void DrawPlayerSpawn()
        {
            if (_tileMap.PlayerSpawn.X < 0)
                return;

            double cx = (_tileMap.PlayerSpawn.X + 0.5) * _tileMap.TileSize;
            double cy = (_tileMap.PlayerSpawn.Y + 0.5) * _tileMap.TileSize;

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
                double cx = (spawn.X + 0.5) * _tileMap.TileSize;
                double cy = (spawn.Y + 0.5) * _tileMap.TileSize;

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

        //Mouse Event Helpers
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

            _tileMap.PlayerSpawn = new Point(x, y);
            DrawGrid();
            return true;
        }

        private bool HandleEnemySpawn(Point pos, MouseButtonEventArgs e)
        {
            if (!TryGetTilePosition(pos, out int x, out int y))
                return false;

            Point p = new Point(x, y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!_tileMap.EnemySpawns.Contains(p))
                    _tileMap.EnemySpawns.Add(p);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _tileMap.EnemySpawns.Remove(p);
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

        private void PaintTile(Point pos) => PaintingHelper(pos, true);

        private void EraseTile(Point pos) => PaintingHelper(pos, false);

        private void Fill(Point pos)
        {
            int startX = (int)(pos.X / _tileMap.TileSize);
            int startY = (int)(pos.Y / _tileMap.TileSize);

            if (startX < 0 || startY < 0 || startX >= _tileMap.Width || startY >= _tileMap.Height)
                return;

            int startIndex = _tileMap.GetIndex(startX, startY);
            int targetValue = _tileMap.Tiles[startIndex];
            int newValue = (int)_currentTileType;

            if (targetValue == newValue)
                return;

            FillFlood(startX, startY, targetValue, newValue);

            if(_currentBatch != null && !_currentBatch.IsEmpty())
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

                if (_tileMap.Tiles[index] != target)
                    continue;

                ApplyTileChange(index, newValue);

                stack.Push((cx + 1, cy));
                stack.Push((cx - 1, cy));
                stack.Push((cx, cy + 1));
                stack.Push((cx, cy - 1));
            }
        }

        private void PaintingHelper(Point pos, bool isPaint)
        {
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

                    if (isPaint)
                    {
                        ApplyTileChange(index, (int)_currentTileType);
                    }
                    else
                    {
                        ApplyTileChange(index, (int)TileType.None);
                    }

                }
            }

            DrawGrid();
        }

        private void ApplyTileChange(int index, int newValue)
        {
            int oldValue = _tileMap.Tiles[index];

            if (oldValue == newValue)
                return;

            var command = new TileChangeCommand(index, oldValue, newValue);

            command.Redo(_tileMap.Tiles);
            _currentBatch?.Add(command);
        }

        //Stack Methods
        private void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var batch = _undoStack.Pop();
            batch.Undo(_tileMap.Tiles);
            _redoStack.Push(batch);

            DrawGrid();
        }

        private void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var batch = _redoStack.Pop();
            batch.Redo(_tileMap.Tiles);
            _undoStack.Push(batch);

            DrawGrid();
        }

        #endregion
    }
}