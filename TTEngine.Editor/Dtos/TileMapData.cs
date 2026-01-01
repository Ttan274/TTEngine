using System.Windows;
using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Dtos
{
    public class TileMapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public Dictionary<MapLayerType, int[]> Layers { get; set; }
        public SpawnDto PlayerSpawn { get; set; }
        public List<SpawnDto> EnemySpawns { get; set; } 
    }
}
