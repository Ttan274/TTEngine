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

namespace TTEngine.Editor.Services.Map
{
    public class MapRenderer
    {
        //Camera State
        public float CameraX { get; set; }
        public float CameraY { get; set; }
        public float Zoom { get; set; } = 1f;

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
        private Rectangle _selectionRect;

        public MapRenderer(Canvas root, EditorState state)
        {
            _root = root;
            _state = state;

            _root.Children.Clear();
            _root.Children.Add(_gridLayer);
            _root.Children.Add(_tileLayer);
            _root.Children.Add(_objectLayer);
            _root.Children.Add(_overlayLayer);

            _hoverRect = new Rectangle();
            _selectionRect = new Rectangle();
            CreateRect(_hoverRect, Brushes.Yellow);
            CreateRect(_selectionRect, Brushes.Purple);
        }

        public void InitializeGrid()
        {
            _gridLayer.Children.Clear();

            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var map = scene.Map;

            //_root.Width = map.Width * map.TileSize;
            //_root.Height = map.Height * map.TileSize;

            double size = ScaledTileSize(map.TileSize);

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Rectangle grid = new Rectangle
                    {
                        Width = size,
                        Height = size,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 0.5 * Zoom,
                        Fill = Brushes.DimGray,
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(grid, WorldToScreenX(x * map.TileSize));
                    Canvas.SetTop(grid, WorldToScreenY(y * map.TileSize));

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

                    double size = ScaledTileSize(map.TileSize);

                    Rectangle rect = new Rectangle
                    {
                        Width = size,
                        Height = size,
                        Stroke = GetTileStroke(tileId),
                        StrokeThickness = 2 * Zoom,
                        Fill = GetTileBrush(tileId),
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(rect, WorldToScreenX(x * map.TileSize));
                    Canvas.SetTop(rect, WorldToScreenY(y * map.TileSize));
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

                string fullPath = System.IO.Path.Combine(_state.Project.TexturesPath, sprite.SpritePath);

                if (!File.Exists(fullPath))
                    continue;

                double screenX = WorldToScreenX(obj.X * map.TileSize);
                double screenY = WorldToScreenY(obj.Y * map.TileSize);

                double size = ScaledTileSize(map.TileSize);

                Image img = new Image
                {
                    Source = GetImage(fullPath),
                    Width = size,
                    Height = size,
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(img, screenX);
                Canvas.SetTop(img, screenY);

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

        #region Hover/Selection

        private void CreateRect(Rectangle rect, Brush b)
        {
            rect.Stroke = b;
            rect.StrokeThickness = 2;
            rect.Fill = Brushes.Transparent;
            rect.IsHitTestVisible = false;
            rect.Visibility = Visibility.Hidden;
            _overlayLayer.Children.Add(rect);
        }

        public void UpdateHover(Point pos, int brushSize)
        {
            var scene = _state.SceneSession.ActiveScene;
            if (scene == null)
                return;

            var map = scene.Map;

            double worldX = pos.X / Zoom + CameraX;
            double worldY = pos.Y / Zoom + CameraY;

            int x = (int)(worldX / map.TileSize);
            int y = (int)(worldY / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
            {
                _hoverRect.Visibility = Visibility.Hidden;
                return;
            }

            _selectionRect.Visibility = Visibility.Hidden;

            double size = ScaledTileSize(map.TileSize);

            _hoverRect.Visibility = Visibility.Visible;
            _hoverRect.Width = brushSize * size;
            _hoverRect.Height = brushSize * size;

            Canvas.SetLeft(_hoverRect, WorldToScreenX(x * map.TileSize));
            Canvas.SetTop(_hoverRect, WorldToScreenY(y * map.TileSize));
        }

        public void UpdateSelection()
        {
            var selected = _state.SceneSession.SelectedObject;
            var scene = _state.SceneSession.ActiveScene;

            if(scene == null || selected == null)
            {
                _selectionRect.Visibility = Visibility.Hidden;
                return; 
            }

            var map = scene.Map;

            double size = ScaledTileSize(map.TileSize);

            _selectionRect.Visibility = Visibility.Visible;
            _selectionRect.Width = size;
            _selectionRect.Height = size;

            Canvas.SetLeft(_selectionRect, WorldToScreenX(selected.X * map.TileSize));
            Canvas.SetTop(_selectionRect, WorldToScreenY(selected.Y * map.TileSize));
        }

        public void OnMouseEnter()
        {
            _selectionRect.Visibility = Visibility.Hidden;
        }

        public void OnMouseLeave()
        {
            _hoverRect.Visibility = Visibility.Hidden;
            UpdateSelection();
        }
        #endregion

        #region Camera Movement

        public void MoveCamera(float dx, float dy)
        {
            CameraX += dx;
            CameraY += dy;
        }

        public void ChangeZoom(float delta)
        {
            Zoom += delta;

            if (Zoom < 0.5f)
                Zoom = 0.5f;

            if (Zoom > 3f)
                Zoom = 3f;
        }

        #endregion

        #region Helpers

        private double WorldToScreenX(double worldX) => (worldX - CameraX) * Zoom;
        private double WorldToScreenY(double worldY) => (worldY - CameraY) * Zoom;
        private double ScaledTileSize(int tileSize) => tileSize * Zoom;

        #endregion
    }
}
