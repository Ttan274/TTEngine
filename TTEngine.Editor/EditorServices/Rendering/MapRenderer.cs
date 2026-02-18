using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.EditorServices.Rendering
{
    public class MapRenderer
    {
        private readonly Canvas _root;
        private readonly EditorState _state;

        //Layers
        private readonly Canvas _gridLayer = new();
        private readonly Canvas _tileLayer = new();
        private readonly Canvas _objectLayer = new();
        private readonly Canvas _overlayLayer = new();

        //Cache
        private readonly Dictionary<string, BitmapImage> _imageCache = new();

        //Hover Rectangle
        private Rectangle _hoverRect;

        public MapRenderer(Canvas root, EditorState state)
        {
            _root = root;
            _state = state;

            _root.Children.Clear();
            _root.Children.Add(_gridLayer);
            _root.Children.Add(_tileLayer);
            _root.Children.Add(_objectLayer);
            _root.Children.Add(_overlayLayer);

            CreateHoverRect();
        }

        public void InitializeGrid()
        {
            _gridLayer.Children.Clear();

            if (_state.MapSession.ActiveMap == null)
                return;

            var map = _state.MapSession.ActiveMap;

            _root.Width = map.Width * map.TileSize;
            _root.Height = map.Height * map.TileSize;

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Rectangle grid = new Rectangle
                    {
                        Width = map.TileSize,
                        Height = map.TileSize,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 0.5,
                        Fill = Brushes.DimGray,
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(grid, x * map.TileSize);
                    Canvas.SetTop(grid, y * map.TileSize);

                    _gridLayer.Children.Add(grid);
                }
            }
        }

        #region Draw Region
        public void DrawStatic()
        {
            _tileLayer.Children.Clear();
            _objectLayer.Children.Clear();

            if (_state.MapSession.ActiveMap == null)
                return;

            DrawLayers();
            DrawPlayerSpawn();
            DrawEnemySpawns();
            DrawInteractables();
            DrawTraps();
        }

        private void DrawLayers()
        {
            foreach (var layer in _state.Layer.Layers)
            {
                if (!layer.IsVisible)
                    continue;

                DrawLayer(layer.LayerType);
            }
        }

        private void DrawLayer(MapLayerType layerType)
        {
            var map = _state.MapSession.ActiveMap;
            var tiles = map.Layers[layerType];

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int index = map.GetIndex(x, y);
                    int tile = tiles[index];

                    if (tile == 0)
                        continue;

                    Rectangle rect = new Rectangle
                    {
                        Width = map.TileSize,
                        Height = map.TileSize,
                        Stroke = GetTileStroke(tile, layerType),
                        StrokeThickness = (layerType == MapLayerType.Collision) ? 2.0 : 0.0,
                        Fill = GetTileBrush(tile, layerType),
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(rect, x * map.TileSize);
                    Canvas.SetTop(rect, y * map.TileSize);
                    _tileLayer.Children.Add(rect);
                }
            }
        }

        private void DrawPlayerSpawn()
        {
            var map = _state.MapSession.ActiveMap;

            if (map.PlayerSpawn == null)
                return;

            double cx = (map.PlayerSpawn.Position.X + 0.5) * map.TileSize;
            double cy = (map.PlayerSpawn.Position.Y + 0.5) * map.TileSize;

            Ellipse marker = new Ellipse
            {
                Width = map.TileSize * 0.6,
                Height = map.TileSize * 0.6,
                Stroke = Brushes.Gold,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(marker, cx - marker.Width / 2);
            Canvas.SetTop(marker, cy - marker.Height / 2);

            _objectLayer.Children.Add(marker);
        }

        private void DrawEnemySpawns()
        {
            var map = _state.MapSession.ActiveMap;

            foreach (var spawn in map.EnemySpawns)
            {
                double cx = (spawn.Position.X + 0.5) * map.TileSize;
                double cy = (spawn.Position.Y + 0.5) * map.TileSize;

                Ellipse marker = new Ellipse
                {
                    Width = map.TileSize * 0.5,
                    Height = map.TileSize * 0.5,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent,
                    IsHitTestVisible = false,
                    Tag = "EnemySpawn"
                };

                Canvas.SetLeft(marker, cx - marker.Width / 2);
                Canvas.SetTop(marker, cy - marker.Height / 2);

                _objectLayer.Children.Add(marker);
            }
        }

        private void DrawInteractables()
        {
            var map = _state.MapSession.ActiveMap;

            //Draw Interactable
            foreach (var interactable in map.Interactables)
            {
                var def = _state.Definition.InteractableDefinitions.FirstOrDefault(d => d.Id == interactable.DefinitionId);

                if (def == null || string.IsNullOrEmpty(def.ImagePath))
                    continue;

                string targetPath = System.IO.Path.Combine(EditorPaths.GetTextureFolder(), def.ImagePath);

                if (!System.IO.File.Exists(targetPath))
                    continue;

                Image img = new Image
                {
                    Source = GetImage(targetPath),
                    Width = map.TileSize,
                    Height = map.TileSize,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, interactable.X * map.TileSize);
                Canvas.SetTop(img, interactable.Y * map.TileSize);
                _objectLayer.Children.Add(img);
            }
        }

        private void DrawTraps()
        {
            var map = _state.MapSession.ActiveMap;

            //Draw Trap
            foreach (var trap in map.Traps)
            {
                var def = _state.Definition.TrapDefinitions.FirstOrDefault(d => d.Id == trap.DefinitionId);

                if (def == null || string.IsNullOrEmpty(def.ImagePath))
                    continue;

                string targetPath = System.IO.Path.Combine(EditorPaths.GetTextureFolder(), def.ImagePath);

                if (!System.IO.File.Exists(targetPath))
                    continue;

                Image img = new Image
                {
                    Source = GetImage(targetPath),
                    Width = map.TileSize,
                    Height = map.TileSize,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, trap.X * map.TileSize);
                Canvas.SetTop(img, trap.Y * map.TileSize);
                _objectLayer.Children.Add(img);
            }
        }
        #endregion

        #region Image Cache

        private BitmapImage GetImage(string path)
        {
            if(!_imageCache.ContainsKey(path))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                _imageCache[path] = bitmap;
            }

            return _imageCache[path];
        }

        #endregion

        #region Paint
        private Brush GetTileStroke(int tile, MapLayerType layer)
        {
            if (layer != MapLayerType.Collision)
                return Brushes.Transparent;

            var def = _state.GetTileById(tile);
            if (def == null)
                return Brushes.Transparent;

            return def.CollisionType switch
            {
                CollisionType.Ground => Brushes.Orange,
                CollisionType.Wall => Brushes.Red,
                _ => Brushes.Transparent
            };
        }

        private Brush GetTileBrush(int tile, MapLayerType layer)
        {
            var def = _state.GetTileById(tile);
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

        #endregion

        #region Hover

        private void CreateHoverRect()
        {
            _hoverRect = new Rectangle
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                IsHitTestVisible = false,
                Visibility = Visibility.Hidden
            };

            _overlayLayer.Children.Add(_hoverRect);
        }

        public void MakeHoverUnvisible()
        {
            _hoverRect.Visibility = Visibility.Hidden;
        }

        public void UpdateHover(Point pos, int brushSize)
        {
            var map = _state.MapSession.ActiveMap;
            if (map == null)
                return;

            int x = (int)(pos.X / map.TileSize);
            int y = (int)(pos.Y / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
            {
                _hoverRect.Visibility = Visibility.Hidden;
                return;
            }

            _hoverRect.Visibility = Visibility.Visible;
            _hoverRect.Width = brushSize * map.TileSize;
            _hoverRect.Height = brushSize * map.TileSize;
            _hoverRect.Stroke = _state.Layer.IsActiveLayerLocked ? Brushes.Gray : Brushes.Yellow;

            Canvas.SetLeft(_hoverRect, x * map.TileSize);
            Canvas.SetTop(_hoverRect, y * map.TileSize);
        }
        #endregion
    }
}
