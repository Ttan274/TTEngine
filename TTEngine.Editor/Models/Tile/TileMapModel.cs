using System.Windows;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Entity;

namespace TTEngine.Editor.Models.Tile
{
    public enum TileType
    {
        None = 0,
        Ground,
        Wall
    }

    public class TileMapModel
    {
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 30;
        public int TileSize { get; set; } = 50;
        public Dictionary<MapLayerType, int[]> Layers { get; set; }
        public PlayerSpawnModel PlayerSpawn { get; set; }
        public List<EnemySpawnModel> EnemySpawns { get; set; } = new();

        public void Init()
        {
            Layers = new Dictionary<MapLayerType, int[]>
            {
                {MapLayerType.Background, CreateEmptyLayer() },
                {MapLayerType.Collision, CreateEmptyLayer() },
                {MapLayerType.Decoration, CreateEmptyLayer() },
            };
        }

        private int[] CreateEmptyLayer()
        {
            var arr = new int[Width * Height];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = (int)TileType.None;

            return arr;
        }

        public int GetIndex(int x, int y) => (y * Width + x);
    }
}
