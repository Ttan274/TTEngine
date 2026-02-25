using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Tile;
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

            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var map = scene.Map;

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

            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            DrawCollisionTiles(scene);
            DrawSpawns(scene);
        }

        private void DrawCollisionTiles(Scene scene)
        {
            var map = scene.Map;

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int tileId = map.CollisionTiles[y][x];

                    if (tileId == 0)
                        continue;

                    Rectangle rect = new Rectangle
                    {
                        Width = map.TileSize,
                        Height = map.TileSize,
                        Stroke = GetTileStroke(tileId),
                        StrokeThickness = 2,
                        Fill = GetTileBrush(tileId),
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(rect, x * map.TileSize);
                    Canvas.SetTop(rect, y * map.TileSize);
                    _tileLayer.Children.Add(rect);
                }
            }
        }

        private void DrawSpawns(Scene scene)
        {
            var map = scene.Map;

            //Player
            if(scene.Spawns.Player != null)
            {
                double cx = (scene.Spawns.Player.X + 0.5) * map.TileSize;
                double cy = (scene.Spawns.Player.Y + 0.5) * map.TileSize;

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

            //Enemies
            foreach(var e in scene.Spawns.Enemies)
            {
                double ex = (e.X + 0.5) * map.TileSize;
                double ey = (e.Y + 0.5) * map.TileSize;

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

                Canvas.SetLeft(marker, ex - marker.Width / 2);
                Canvas.SetTop(marker, ey - marker.Height / 2);

                _objectLayer.Children.Add(marker);
            }

            //Interactables-Traps
            DrawInteractables(scene, map);            
        }

        private void DrawInteractables(Scene scene, MapData map)
        {
            //Draw Interactables
            foreach (var interactable in scene.Spawns.Interactables)
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

            //Draw Traps
            foreach (var trap in scene.Spawns.Traps)
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
        private Brush GetTileStroke(int tileId)
        {
            var def = _state.GetTileById(tileId);
            if (def == null)
                return Brushes.Transparent;

            return def.CollisionType switch
            {
                CollisionType.Ground => Brushes.Orange,
                CollisionType.Wall => Brushes.Red,
                _ => Brushes.Transparent
            };
        }

        private Brush GetTileBrush(int tileId)
        {
            var def = _state.GetTileById(tileId);
            if (def == null)
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
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var map = scene.Map;

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

            Canvas.SetLeft(_hoverRect, x * map.TileSize);
            Canvas.SetTop(_hoverRect, y * map.TileSize);
        }
        #endregion
    }
}
