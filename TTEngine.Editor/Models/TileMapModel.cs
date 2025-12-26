
using System.Windows;

namespace TTEngine.Editor.Models
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
        public int[] Tiles { get; set; }
        public Point PlayerSpawn { get; set; } = new Point(-1, -1);
        public List<Point> EnemySpawns { get; set; } = new();

        public void Init()
        {
            Tiles = new int[Width * Height];
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = (int)TileType.None;
        }

        public int GetIndex(int x, int y) => (y * Width + x);
    }
}
