using System.Windows;

namespace TTEngine.Editor.Dtos
{
    public class TileMapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public int[] Tiles { get; set; }
        public SpawnDto PlayerSpawn { get; set; }
        public List<SpawnDto> EnemySpawns { get; set; } 
    }
}
