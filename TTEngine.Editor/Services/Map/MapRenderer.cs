using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Services.Map
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
            DrawSceneObjects(scene);
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

        private void DrawSceneObjects(Scene scene)
        {
            var map = scene.Map;

            foreach (var obj in scene.SceneObjects)
            {
                if (!obj.IsActive)
                    continue;

                var prefab = _state.Definition.GameObjects
                    .FirstOrDefault(p => p.Id == obj.PrefabId);

                if (prefab == null)
                    continue;

                var sprite = prefab.Components.
                    OfType<SpriteRendererComponent>().FirstOrDefault();

                if (sprite == null || string.IsNullOrEmpty(sprite.SpritePath))
                    continue;

                string fullPath = System.IO.Path.Combine(EditorPaths.GetTextureFolder(), sprite.SpritePath);

                if (!File.Exists(fullPath))
                    continue;

                double x = obj.X * map.TileSize;
                double y = obj.Y * map.TileSize;

                Image img = new Image
                {
                    Source = GetImage(fullPath),
                    Width = map.TileSize,
                    Height = map.TileSize,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, x);
                Canvas.SetTop(img, y);

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
