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
        private const string LIVE_MAP_PATH = @"..\..\..\..\Maps\active_map.json";
        private const string ENGINE_PATH = @"..\..\..\..\x64\Debug\TTEngine.exe";

        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Undo,
                (_, _) => Undo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Redo,
                (_, _) => Redo()
            ));

            BrushSizeSlider.Value = 1;
            _tileMap.Init();
            
            DrawGrid();
            CreateHoverRect();
        }

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
        }
    
        //Mouse Events
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPainting = true;
            Mouse.Capture(MapCanvas);

            Point pos = e.GetPosition(MapCanvas);

            //Player Spawn Tool
            if (_currentToolMode == ToolMode.Spawn && e.LeftButton == MouseButtonState.Pressed)
            {
                int tileX = (int)(pos.X / _tileMap.TileSize);
                int tileY = (int)(pos.Y / _tileMap.TileSize);

                if (tileX < 0 || tileY < 0 ||
                tileX >= _tileMap.Width || tileY >= _tileMap.Height)
                {
                    Mouse.Capture(null);
                    _isPainting = false;
                    return;
                }

                _tileMap.PlayerSpawn = new Point(tileX, tileY);

                DrawGrid();

                Mouse.Capture(null);
                _isPainting = false;
                return;
            }


            //Fill and Brush Tools
            _currentBatch = new TileBatchCommand();

            if(_currentToolMode == ToolMode.Fill && e.LeftButton == MouseButtonState.Pressed)
            {
                Fill(pos);

                Mouse.Capture(null);
                _isPainting = false;
                return;
            }
            else
            {
                if (e.RightButton == MouseButtonState.Pressed)
                    EraseTile(e.GetPosition(MapCanvas));
                else if (e.LeftButton == MouseButtonState.Pressed)
                    PaintTile(e.GetPosition(MapCanvas));
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

        //Text Events
        private void BrushSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BrushSizeLabel == null)
                return;

            _brushSize = (int)e.NewValue;
            BrushSizeLabel.Text = $"Size: {_brushSize}x{_brushSize}";
        }

        //Button Events
        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            var data = new TileMapData
            {
                Width = _tileMap.Width,
                Height = _tileMap.Height,
                TileSize = _tileMap.TileSize,
                Tiles = _tileMap.Tiles,
                PlayerSpawnX = _tileMap.PlayerSpawn.X,
                PlayerSpawnY = _tileMap.PlayerSpawn.Y
            };

            MapFileService.Save(LIVE_MAP_PATH, data);
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            TileMapData data = MapFileService.Load(LIVE_MAP_PATH);

            _tileMap.Width = data.Width;
            _tileMap.Height = data.Height;
            _tileMap.TileSize = data.TileSize;
            _tileMap.Tiles = data.Tiles;
            _tileMap.PlayerSpawn = new Point(data.PlayerSpawnX, data.PlayerSpawnY);

            DrawGrid();
        }

        private void BrushTool_Checked(object sender, RoutedEventArgs e) => _currentToolMode = ToolMode.Brush;

        private void FillTool_Checked(object sender, RoutedEventArgs e) => _currentToolMode = ToolMode.Fill;

        private void SpawnTool_Checked(object sender, RoutedEventArgs e) => _currentToolMode = ToolMode.Spawn;

        private void StartGame_Click(object sender, RoutedEventArgs e) => Process.Start(ENGINE_PATH, LIVE_MAP_PATH);


        //Methods
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
            int newValue = WallRadio.IsChecked == true ? (int)TileType.Wall :
                           GroundRadio.IsChecked == true ? (int)TileType.Ground : targetValue;

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

        //Helper methods
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

                    if(isPaint)
                    {
                        if (WallRadio.IsChecked == true)
                            ApplyTileChange(index, (int)TileType.Wall);
                        else if (GroundRadio.IsChecked == true)
                            ApplyTileChange(index, (int)TileType.Ground);
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
    }
}